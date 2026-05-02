# MeteoApp — Report di Progetto

## 1. Introduzione

MeteoApp è un'applicazione meteo mobile multipiattaforma sviluppata con .NET MAUI, che supporta Android, iOS e Windows da un'unica codebase condivisa. L'app consente agli utenti di salvare più località, visualizzare le condizioni meteo in tempo reale e le previsioni per le prossime 24 ore per ciascuna di esse, e mantenere la lista delle località sincronizzata tra dispositivi tramite un backend cloud.

**Obiettivi principali:**

- Offrire un'esperienza meteo pulita e reattiva con condizioni attuali e previsioni multi-giorno.
- Persistere le località dell'utente localmente per l'accesso offline e sincronizzarle con il backend remoto quando la connettività è disponibile.
- Supportare le notifiche push per avvisare gli utenti delle condizioni meteo su base programmata.
- Dimostrare un'architettura MVVM ben strutturata con una netta separazione tra il layer di dominio indipendente dalla piattaforma e il layer specifico per MAUI.

---

## 2. Tecnologie e Metodologie

### Framework & Linguaggio

| Tecnologia | Ruolo |
|---|---|
| .NET 10 / C# | Linguaggio principale e runtime |
| .NET MAUI | Framework UI multipiattaforma (Android, iOS, Windows) |
| Blazor WebView | Componente web incorporato per i grafici interattivi delle previsioni |
| MAUI Maps | Mappa interattiva per la ricerca e selezione delle località |

### Backend & Dati

| Tecnologia | Ruolo |
|---|---|
| OpenWeatherMap API | Dati meteo attuali e previsioni a 5 giorni/3 ore |
| Appwrite | Database cloud per la sincronizzazione delle località e dei token FCM |
| SQLite (`sqlite-net-pcl`) | Storage locale persistente per l'accesso offline |
| Firebase Cloud Messaging | Distribuzione delle notifiche push ai dispositivi registrati |

### Strumenti & Testing

| Tecnologia | Ruolo |
|---|---|
| xUnit | Framework per i test unitari |
| Moq | Libreria di mocking per isolare le unità in test |
| coverlet | Raccolta della copertura del codice |

### Funzione Serverless

Una funzione schedulata Node.js distribuita su Appwrite (`function/src/main.js`) gestisce l'invio server-side delle notifiche push. Utilizza `firebase-admin` per inviare messaggi FCM a tutti i token dei dispositivi registrati nel database Appwrite.

### Metodologie

- **MVVM (Model-View-ViewModel)**: Tutto lo stato e la logica dell'interfaccia risiedono nei ViewModel che implementano `INotifyPropertyChanged`. Le View sono XAML puro senza logica nel code-behind, collegate interamente tramite data binding e `RelayCommand`.
- **Repository pattern con astrazione di sincronizzazione**: Tre interfacce (`IDatabase<T>`, `ISyncableLocalDatabase<T>`, `ISyncableRemoteDatabase<T>`) definiscono un contratto uniforme per l'accesso ai dati, permettendo al layer di orchestrazione della sync di operare indipendentemente dall'implementazione dello storage.
- **Dependency Injection**: L'intero grafo degli oggetti è configurato in `MauiProgram.cs` usando il container `Microsoft.Extensions.DependencyInjection`. Le interfacce del Core vengono risolte a runtime con le loro implementazioni specifiche per MAUI.
- **Soft-delete pattern**: I record non vengono mai rimossi fisicamente dal database locale o remoto; vengono contrassegnati con `IsDeleted = true` e filtrati in lettura. Questo garantisce la correttezza della sincronizzazione quando una cancellazione deve propagarsi agli altri dispositivi.

---

## 3. Processo di Implementazione e Principali Sfide

### Struttura della Soluzione

La soluzione è suddivisa in quattro progetti per garantire la separazione delle responsabilità:

- **`MeteoApp.Core`** — Layer di dominio indipendente dalla piattaforma: interfacce, modelli, ViewModel base, helper. Nessuna dipendenza da MAUI.
- **`MeteoApp`** — Layer MAUI: implementazioni concrete dei servizi, view XAML, configurazione della DI.
- **`MeteoApp.Core.Tests`** — Test unitari XUnit che coprono ViewModel, Services, Models e Helpers.
- **`function/`** — Funzione serverless Node.js su Appwrite per le notifiche push schedulate.

### Motore di Sincronizzazione

La parte più complessa dell'implementazione è il servizio di sincronizzazione bidirezionale (`AppwriteSyncService`). Gestisce tre strategie di risoluzione dei conflitti configurabili dall'utente a runtime:

- **`LocalWins`**: Le modifiche locali vengono caricate per prime; le modifiche remote vengono applicate solo se non esiste un record locale corrispondente.
- **`RemoteWins`**: Le modifiche remote vengono scaricate e sovrascrivono lo stato locale in modo incondizionato.
- **`LatestWins`** (predefinita): Vince il record con il timestamp `UpdatedAt` più recente.

La sincronizzazione incrementale è ottenuta persistendo un timestamp `lastSyncDate` nelle MAUI Preferences, in modo da recuperare da Appwrite solo i record modificati dall'ultima sincronizzazione, riducendo banda e latenza.

**Principali sfide affrontate:**

- **Sessioni anonime Appwrite**: Il client Appwrite richiede una sessione attiva anche per l'accesso non autenticato. La sessione deve essere creata al primo utilizzo e riutilizzata nelle richieste successive; dimenticare di inizializzarla causa il fallimento silenzioso di tutte le chiamate remote.
- **Race create-or-update su Appwrite**: Appwrite restituisce HTTP 409 quando si tenta di creare un documento con un ID già esistente. L'implementazione del database remoto intercetta questo conflitto e ricade su una chiamata di aggiornamento, rendendo le operazioni upsert idempotenti.
- **Inizializzazione di SQLite multipiattaforma**: La configurazione di SQLite differisce tra Android, iOS e Windows. Il percorso del database deve essere risolto a runtime tramite `FileSystem.AppDataDirectory` e i binari nativi corretti (`SQLitePCLRaw.bundle_green`) devono essere collegati per ciascuna piattaforma target.
- **Blazor WebView in MAUI**: Incorporare un componente Blazor per il grafico delle previsioni richiede l'abilitazione del sottosistema Blazor hybrid in `MauiProgram.cs` e il corretto routing degli asset. Il ciclo di vita della WebView è legato a quello della pagina MAUI, il che richiede una gestione attenta dello stato.
- **Cambio lingua a runtime**: Modificare la lingua dell'app richiede la ricostruzione dell'`AppShell` e la ri-registrazione di tutte le route a runtime, poiché MAUI non ricarica dinamicamente i dizionari di risorse al cambio di `CultureInfo`.

### Notifiche Push

I token FCM dei dispositivi vengono registrati all'avvio dell'app tramite `NotificationProvider` e salvati in una collezione dedicata di Appwrite. La funzione schedulata di Appwrite interroga tutti i token e invia le notifiche server-side, disaccoppiando la programmazione delle notifiche dalla disponibilità online di qualsiasi singolo dispositivo.

---

## 4. Valutazione e Possibili Miglioramenti

### Risultato

L'applicazione raggiunge i suoi obiettivi principali: i dati meteo vengono recuperati e visualizzati in modo affidabile, il database locale garantisce un'esperienza offline funzionante, e il motore di sincronizzazione risolve correttamente i conflitti con le strategie supportate. L'architettura è pulita e testabile — la separazione tra `MeteoApp.Core` e `MeteoApp` permette di eseguire i test unitari sulla logica di dominio senza un dispositivo o un emulatore in esecuzione.

### Possibili Miglioramenti

| Area | Miglioramento |
|---|---|
| **Autenticazione** | Sostituire le sessioni anonime di Appwrite con account utente reali, abilitando liste di località per utente e sincronizzazione multi-dispositivo senza condividere un namespace globale. |
| **Contenuto delle notifiche push** | Il payload attuale delle notifiche è statico ("Guarda la meteo — È importante"). Potrebbe essere arricchito con dati meteo reali recuperati al momento dell'invio (es. temperatura, condizioni di allerta). |
| **Gestione degli errori offline-first** | I fallimenti di rete durante la sync vengono registrati nei log ma non vengono mostrati all'utente. Un indicatore visibile dello stato della sync con supporto al retry migliorerebbe la robustezza. |
| **Grafico delle previsioni** | Il grafico Blazor WebView funziona ma aggiunge overhead all'avvio e uno stack di rendering separato. Sostituirlo con una libreria di grafici nativa MAUI (es. Microcharts o LiveCharts2) ridurrebbe la complessità e migliorerebbe le prestazioni. |
| **Copertura dei test** | I test di integrazione per il servizio di sync contro un'istanza Appwrite reale (o containerizzata) intercetterebbero le incompatibilità di contratto che i test unitari con mock non riescono a rilevare. |
| **CI/CD** | Una pipeline GitHub Actions per compilare, eseguire i test e produrre artefatti APK/IPA firmati ad ogni push automatizzerebbe i controlli di qualità. |
