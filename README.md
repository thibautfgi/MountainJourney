<h1 align="center">MountainJourney</h1>

<p align="center">
  <img src="/SITE_MJ/Front/ImageRessource/logo-transp.png.png" alt="MountainJourney Logo" width="200">
</p>

## Description

MountainJourney is the new web application for create manage and share your hiking map for your next trip. MJ have a API for manage users information and hiking map, and use the frontend API of Mapbox for create the map.

This is a project free of right. You can use it has you wish if you integre our names. Thanks for your reading.

## Needs

- Docker.
- MySql.
- Dotnet.
- Mysql workbench and Postman agent (for local test).

## Installation

Clone the repot and explore the project directory :

```bash
git clone <https://github.com/DuunKy/MontainJourney.git>
cd MontainJourney


``` 

## Local test of our project :


For test our project on local, use the command bellow :

```bash
dotnet run --project SITE_MJ/API_MJ/
```

## Use Postman
Try it on Postamn for a better view of our API capabilities

[Workspace Postman](https://blue-comet-541359.postman.co/workspace/Mountain-Journey~ef3aa6a7-6af4-474c-9a98-4e77433ef51b/overview)

## API Bearer Tokens

Here our bearer Tokens for the protect request on the API :

```bash
abcdef123456
```


>**Warning :**

The mapbox tokens is hiding on a .gitignore file. For all acces, you will need to ask the keys to a admin.

## Lauch the application with Docker Compose :

For lauch the web application, use the command below :

```bash
docker-compose up --build
```

## Shut down the application :

For stop the service, use the command :

```bash
docker-compose down
```



># ** WARNING : ** Sometime, git can modify the sh executable endline and break it. If that happen, you will need to change your encoding of your sh file from CRLF -> LF (bottom right corner of VS CODE).

## Auteurs

 - ThibautFgi (Github : https://github.com/thibautfgi)
 - DunKy (Github : https://github.com/DuunKy)

# **Documentationn : ** 

For a more advance documentation on the web application look into the folder :

- "Documentation/."

### DMC

<p align="center">
  <img src="/Documentation/MJ_DMC.png" alt="MountainJourney Logo">
</p>


