#!/bin/bash
# Démarrer l'application .NET en arrière-plan
dotnet run --project /API/ &

# Démarrer NGINX en avant-plan
nginx -g 'daemon off;'
