# Importing an IA model to a BLiP Chatbot

This project help you to import entities and intentions from a csv file to a BLiP chatbot 

## Prerequisites:

1. Get identifier and accessKey of your bot on BLiP portal (if you have a Webhook chatbot [click here](#getting-identifier-from-webhook) to know how to get this informations).
2. Create a `.csv` file with your intentions ([click here](FileSamples/intentions.csv) to see a sample file)
3. Create a `.csv` file with your entities ([click here](FileSamples/entities.csv) to see a sample file)

## How to use:

1. After get the step 1 informations (prerequisites), open the `application.json` file and set the identifier and accessKey propertly
2. Update `intentionsFilePath` and `entitiesFilePath` variables on `application.json` file with the propertly path of your files (created on prerequisites steps 2 and 3)
3. Run the application

## Misc

### Getting identifier from webhook

To get a Webhook's bot indentifier and accessKey is simple.

1. Go to BLiP portal and get the authorization key value of authentication header. For instance `d2ViaG9va3Rvc2RrOlZpZnpFaTNqdjZ5ZEhhS3UzWWJJ`
2. Make a base64 decode on this value. For instance: decodeBase64('d2ViaG9va3Rvc2RrOlZpZnpFaTNqdjZ5ZEhhS3UzWWJJ') => `webhooktosdk:VifzEi3jv6ydHaKu3YbI`
3. The result of step 3 has two values separeted by ':' (cólon). The value before cólon is the bot identifier
4. Apply a base64 encode on the value after the cólon and use the result as your accessKey. For instance: encodeBase64('VifzEi3jv6ydHaKu3YbI') => `VmlmekVpM2p2NnlkSGFLdTNZYkk=`