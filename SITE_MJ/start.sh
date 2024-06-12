#!/bin/bash
# Démarre l'application .NET en arrière-plan
dotnet run --project /API/ &

# Démarre NGINX en avant-plan
nginx -g 'daemon off;'
