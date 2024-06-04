FROM duunky/montainjourney:BDD-MJ
WORKDIR /app
COPY . /app 
ADD lancement.sql /docker-entrypoint-initdb.d
EXPOSE 3306