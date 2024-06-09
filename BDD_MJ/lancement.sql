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
    User_Phone VARCHAR(80),
    User_Email VARCHAR(80) NOT NULL,
    User_Password VARCHAR(80) NOT NULL,
    PRIMARY KEY (`User_Id`)
);

LOCK TABLES `users` WRITE;

INSERT INTO users (User_FirstName, User_LastName, User_Phone, User_Email, User_Password)
VALUES
('John', 'Doe', '1234567890', 'john.doe@example.com', 'password123'),
('Jane', 'Smith', '2345678901', 'jane.smith@example.com', 'password456'),
('Alice', 'Johnson', '3456789012', 'alice.johnson@example.com', 'password789'),
('Bob', 'Brown', '4567890123', 'bob.brown@example.com', 'password101'),
('Carol', 'Davis', '5678901234', 'carol.davis@example.com', 'password102'),
('David', 'Evans', '6789012345', 'david.evans@example.com', 'password103'),
('Eve', 'Garcia', '7890123456', 'eve.garcia@example.com', 'password104'),
('Frank', 'Harris', '8901234567', 'frank.harris@example.com', 'password105'),
('Grace', 'Lee', '9012345678', 'grace.lee@example.com', 'password106'),
('Hank', 'Martinez', '0123456789', 'hank.martinez@example.com', 'password107');

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
(2, 'Forest Path', 'A serene path through the forest.', 10, 5, 2.0, 8.0, 'forest.jpg', 5),
(3, 'Desert Walk', 'A challenging walk through the desert.', 3, 1, 4.0, 15.0, 'desert.jpg', 3),
(4, 'City Tour', 'A tour through the historic city.', 8, 3, 2.5, 10.0, 'city.jpg', 4),
(5, 'Beachside Path', 'A relaxing path along the beach.', 12, 7, 3.0, 9.0, 'beach.jpg', 5),
(6, 'River Route', 'A route following the river.', 4, 2, 5.0, 20.0, 'river.jpg', 3),
(7, 'Hill Climb', 'A steep climb up the hill.', 6, 4, 2.5, 7.0, 'hill.jpg', 4),
(8, 'Park Loop', 'A loop around the city park.', 9, 6, 1.5, 5.0, 'park.jpg', 5),
(9, 'Countryside Trek', 'A trek through the countryside.', 7, 3, 6.0, 18.0, 'countryside.jpg', 4),
(10, 'Lakeside Walk', 'A walk around the lake.', 11, 8, 2.0, 8.0, 'lake.jpg', 5);

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
(2, 'Old Oak Tree', 'Oak', 39.7392, -105.0003),
(2, 'River Bend', 'Bend', 39.7292, -104.9803),
(3, 'Dune Summit', 'Summit', 39.7192, -105.0103),
(3, 'Oasis', 'Oasis', 39.7092, -104.9703),
(4, 'Historic Plaza', 'Plaza', 39.7992, -104.9503),
(4, 'Museum Entrance', 'Museum', 39.8092, -104.9403),
(5, 'Beach Start', 'Beachhead', 39.8192, -104.9303),
(5, 'Cliff View', 'Cliff', 39.8292, -104.9203);

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
(2, 3, 'Trail Segment 2', 'The second segment of the trail.', 7.0),
(3, 4, 'Desert Route 1', 'The first desert segment.', 6.0),
(4, 5, 'Desert Route 2', 'The second desert segment.', 9.0),
(5, 6, 'City Route 1', 'The first city segment.', 2.0),
(6, 7, 'City Route 2', 'The second city segment.', 4.0),
(7, 8, 'Beach Route 1', 'The first beach segment.', 3.0),
(8, 9, 'Beach Route 2', 'The second beach segment.', 5.0),
(9, 10, 'Hill Route 1', 'The first hill segment.', 2.0),
(10, 1, 'Hill Route 2', 'The second hill segment.', 7.0);

UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "likes"
-- !!!!!!!!!!!!!!!!!!!!!!!
DROP TABLE IF EXISTS `likes`;

CREATE TABLE `likes` (
    Like_Id int NOT NULL AUTO_INCREMENT,
    User_Id INTEGER NOT NULL,
    Map_Id INTEGER NOT NULL,
    PRIMARY KEY (`Like_Id`),
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
(1, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6),
(7, 7),
(8, 8),
(9, 9);

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
(2, 2, 'Very peaceful walk.', '2024-06-02'),
(3, 3, 'A bit too hot.', '2024-06-03'),
(4, 4, 'Loved the historic sights.', '2024-06-04'),
(5, 5, 'The beach was amazing!', '2024-06-05'),
(6, 6, 'Nice river views.', '2024-06-06'),
(7, 7, 'Quite a climb.', '2024-06-07'),
(8, 8, 'The park was beautiful.', '2024-06-08'),
(9, 9, 'Peaceful countryside trek.', '2024-06-09'),
(10, 10, 'The lake walk was so relaxing.', '2024-06-10');

UNLOCK TABLES;

-- !!!!!!!!!!!!!!!!!!!!!!!
-- Table "friendlists"
-- !!!!!!!!!!!!!!!!!!!!!!!
DROP TABLE IF EXISTS `friendlists`;

CREATE TABLE friendlists (
    Friendlist_Id int NOT NULL AUTO_INCREMENT,
    User_Main_Id INTEGER NOT NULL,
    User_Id INTEGER NOT NULL,
    PRIMARY KEY (`Friendlist_Id`),
    FOREIGN KEY (User_Main_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE,
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE
);

LOCK TABLES `friendlists` WRITE;

INSERT INTO friendlists (User_Main_Id, User_Id)
VALUES
(1, 2),
(2, 1),
(3, 4),
(4, 3),
(5, 6),
(6, 5),
(7, 8),
(8, 7),
(9, 10),
(10, 9);

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
(2, 'ghijkl789012'),
(3, 'mnopqr345678'),
(4, 'stuvwx901234'),
(5, 'yzabcd567890'),
(6, 'efghij123456'),
(7, 'klmnop789012'),
(8, 'qrstuv345678'),
(9, 'wxyzab901234'),
(10, 'cdefgh567890');

UNLOCK TABLES;
