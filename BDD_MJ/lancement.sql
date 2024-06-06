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
    User_Phone BIGINT, -- Correction du type de données
    User_Email VARCHAR(80) NOT NULL,
    User_Password VARCHAR(80) NOT NULL,
    PRIMARY KEY (`User_Id`)
);

LOCK TABLES `users` WRITE;

INSERT INTO users (User_FirstName, User_LastName, User_Phone, User_Email, User_Password)
VALUES
('John', 'Doe', 1234567890, 'john.doe@example.com', 'password123'),
('Jane', 'Smith', 2345678901, 'jane.smith@example.com', 'password456');

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

INSERT INTO maps (User_Id, Map_Name, Map_Description, Map_LikeNumber, Map_NumberCommentary, Map_TravelTime, Map_TotalDistance, Map_Image, Map_Rating)
VALUES
(1, 'Mountain Trail', 'A scenic mountain trail.', 5, 2, 3.5, 12.0, 'trail.jpg', 4),
(2, 'Forest Path', 'A serene path through the forest.', 10, 5, 2.0, 8.0, 'forest.jpg', 5);

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

INSERT INTO marks (Map_Id, Mark_Description, Mark_Name, Mark_Latitude, Mark_Longitude)
VALUES
(1, 'Starting Point', 'Trailhead', 39.7392, -104.9903),
(1, 'Scenic Overlook', 'Overlook', 39.7492, -104.9793),
(2, 'Old Oak Tree', 'Oak', 39.7392, -105.0003);

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

INSERT INTO routes (Mark_Start, Mark_End, Route_Name, Route_Description, Route_Distance)
VALUES
(1, 2, 'Trail Segment 1', 'The first segment of the trail.', 5.0),
(2, 3, 'Trail Segment 2', 'The second segment of the trail.', 7.0);

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

INSERT INTO likes (User_Id, Map_Id)
VALUES
(1, 1),
(2, 2),
(1, 2);

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

INSERT INTO comments (User_Id, Map_Id, Comment_Content, Comment_Date)
VALUES
(1, 1, 'Great trail, loved it!', '2024-06-01'),
(2, 2, 'Very peaceful walk.', '2024-06-02');

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

INSERT INTO friendlists (User_Main_Id, User_Id)
VALUES
(1, 2),
(2, 1);

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

INSERT INTO tokens (User_Id, Token_Value)
VALUES
(1, 'abcdef123456'),
(2, 'ghijkl789012');

UNLOCK TABLES;


-- Création d'un utilisateur pour l'API avec des privilèges limités
CREATE USER 'api'@'%' IDENTIFIED BY 'Prout123';

-- Accorder des privilèges de lecture et écriture sur toutes les tables
GRANT SELECT, INSERT, UPDATE, DELETE ON MountainJourney.* TO 'api'@'%';

-- Appliquer les modifications
FLUSH PRIVILEGES;
