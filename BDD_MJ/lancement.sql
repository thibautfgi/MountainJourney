-- Utilisation de la base de données
-- USE MountainJourney;

-- Création des tables

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "users"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `users`;

CREATE TABLE users (
    User_Id int NOT NULL AUTO_INCREMENT,
    User_FirstName VARCHAR(80),
    User_LastName VARCHAR(80),
    User_Phone INTEGER,
    User_Email VARCHAR(80) NOT NULL,
    User_Password VARCHAR(80) NOT NULL,
    PRIMARY KEY (`User_Id`)
);

LOCK TABLES `users` WRITE;
/* INSERT INTO `users` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "maps"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `maps`;

CREATE TABLE maps (
    Map_Id int NOT NULL AUTO_INCREMENT,
    User_Id INTEGER NOT NULL,
    Map_Name VARCHAR(80) NOT NULL,
    Map_Description VARCHAR(120),
    Map_LikeNumber INTEGER,
    Map_NumberCommentary INTEGER,
    Map_TravelTime FLOAT,
    Map_TotalDistance FLOAT,
    Map_Image VARCHAR(80),
    Map_Rating INTEGER,
    PRIMARY KEY (`Map_Id`),
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE
);

LOCK TABLES `maps` WRITE;
/* INSERT INTO `maps` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "marks"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `marks`;

CREATE TABLE marks (
    Mark_Id int NOT NULL AUTO_INCREMENT,
    Map_Id INTEGER NOT NULL,
    Mark_Description VARCHAR(120),
    Mark_Name VARCHAR(80),
    Mark_Latitude FLOAT,
    Mark_Longitude FLOAT,
    PRIMARY KEY (`Mark_Id`),
    FOREIGN KEY (Map_Id) REFERENCES maps(Map_Id)
        ON DELETE CASCADE
);

LOCK TABLES `marks` WRITE;
/* INSERT INTO `marks` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "routes"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `routes`;

CREATE TABLE routes (
    Route_Id int NOT NULL AUTO_INCREMENT,
    Mark_Start INTEGER NOT NULL,
    Mark_End INTEGER NOT NULL,
    Route_Name VARCHAR(80) NOT NULL,
    Route_Description VARCHAR(120),
    Route_Distance FLOAT,
    PRIMARY KEY (`Route_Id`),
    FOREIGN KEY (Mark_Start) REFERENCES marks(Mark_Id)
        ON DELETE CASCADE,
    FOREIGN KEY (Mark_End) REFERENCES marks(Mark_Id)
        ON DELETE CASCADE
);

LOCK TABLES `routes` WRITE;
/* INSERT INTO `routes` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "likes"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `likes`;

CREATE TABLE `likes` (
    User_Id INTEGER NOT NULL,
    Map_Id INTEGER NOT NULL,
    PRIMARY KEY (User_Id, Map_Id),
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE,
    FOREIGN KEY (Map_Id) REFERENCES maps(Map_Id)
        ON DELETE CASCADE
);

LOCK TABLES `likes` WRITE;
/* INSERT INTO `likes` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "comments"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `comments`;

CREATE TABLE comments (
    Comment_Id int NOT NULL AUTO_INCREMENT,
    User_Id INTEGER NOT NULL,
    Map_Id INTEGER NOT NULL,
    Comment_Content VARCHAR(120) NOT NULL,
    Comment_Date VARCHAR(80) NOT NULL,
    PRIMARY KEY (`Comment_Id`),
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE,
    FOREIGN KEY (Map_Id) REFERENCES maps(Map_Id)
        ON DELETE CASCADE
);

LOCK TABLES `comments` WRITE;
/* INSERT INTO `comments` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "friendlists"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `friendlists`;

CREATE TABLE friendlists (
    User_Main_Id INTEGER NOT NULL,
    User_Id INTEGER NOT NULL,
    PRIMARY KEY (User_Main_Id, User_Id),
    FOREIGN KEY (User_Main_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE,
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE
);

LOCK TABLES `friendlists` WRITE;
/* INSERT INTO `friendlists` VALUES (); */
UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "tokens"
-- !!!!!!!!!!!!!!!!!!!!!!!

DROP TABLE IF EXISTS `tokens`;

CREATE TABLE tokens (
    Token_Id int NOT NULL AUTO_INCREMENT,
    User_Id INTEGER NOT NULL,
    Token_Value VARCHAR(80),
    PRIMARY KEY (`Token_Id`),
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE
);

LOCK TABLES `tokens` WRITE;
/* INSERT INTO `tokens` VALUES (); */
UNLOCK TABLES;

-- Création d'un utilisateur pour l'API avec des privilèges limités
CREATE USER 'api'@'%' IDENTIFIED BY 'Prout123';

-- Accorder des privilèges de lecture et écriture sur toutes les tables
GRANT SELECT, INSERT, UPDATE, DELETE ON MountainJourney.* TO 'api'@'%';

-- Appliquer les modifications
FLUSH PRIVILEGES;
