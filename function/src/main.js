import { Client, Databases, Query } from 'node-appwrite';
import firebaseAdmin from 'firebase-admin';
import { createRequire } from 'module';

const require = createRequire(import.meta.url);
const serviceAccount = require('./serviceAccountKey.json');

// Inizializza Firebase una sola volta
if (!firebaseAdmin.apps.length) {
    firebaseAdmin.initializeApp({
        credential: firebaseAdmin.credential.cert(serviceAccount),
    });
}

function sendNotification(registrationToken) {
    const message = {
        token: registrationToken,
        notification: {
            title: 'Guarda la meteo',
            body: 'È importante',
        },
    };

    return firebaseAdmin.messaging().send(message).catch((err) => {
        console.log('Error sending Notification to token', registrationToken, err);
    });
}

export default async ({ req, res, log, error }) => {
    // Consenti l'invocazione SOLO dallo scheduler di Appwrite
    if (req.headers['x-appwrite-trigger'] !== 'schedule') {
        error('Unauthorized: questa funzione può essere richiamata solo dallo scheduler');
        return res.json({ success: false, error: 'Unauthorized' }, 401);
    }

    const client = new Client()
        .setEndpoint(process.env.APPWRITE_FUNCTION_API_ENDPOINT)
        .setProject(process.env.APPWRITE_FUNCTION_PROJECT_ID)
        .setKey(req.headers['x-appwrite-key']);

    const { APPWRITE_DATABASE_ID, APPWRITE_COLLECTION_ID } = process.env;

    try {
        const database = new Databases(client);

        log("Leggo i vari token dal database...");

        const { documents: allDocs } = await database.listDocuments(
            APPWRITE_DATABASE_ID,
            APPWRITE_COLLECTION_ID,
            [Query.limit(1000)]
        );

        log(`Invio notifica a ${allDocs.length} token`);

        await Promise.all(allDocs.map((doc) => sendNotification(doc.token)));

        return res.json({ success: true, count: allDocs.length });
    } catch (err) {
        error(err.message);
        return res.json({ success: false, error: err.message }, 500);
    }
};