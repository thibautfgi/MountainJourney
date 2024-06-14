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
('Hank', 'Martinez', '0123456789', 'hank.martinez@example.com', 'password107'),
('Ivy', 'Taylor', '1234509876', 'ivy.taylor@example.com', 'password108'),
('Jack', 'Anderson', '2345609876', 'jack.anderson@example.com', 'password109'),
('Karen', 'Thomas', '3456709876', 'karen.thomas@example.com', 'password110'),
('Leo', 'Walker', '4567809876', 'leo.walker@example.com', 'password111'),
('Mia', 'Robinson', '5678909876', 'mia.robinson@example.com', 'password112');

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
    Map_Image VARCHAR(255),
    Map_Rating INTEGER,
    PRIMARY KEY (`Map_Id`),
    FOREIGN KEY (User_Id) REFERENCES users(User_Id)
        ON DELETE CASCADE
);

LOCK TABLES `maps` WRITE;

INSERT INTO maps (User_Id, Map_Name, Map_Description, Map_LikeNumber, Map_NumberCommentary, Map_TravelTime, Map_TotalDistance, Map_Image, Map_Rating)
VALUES
(1, 'Mountain Trail', 'A scenic mountain trail.', 5, 2, 3.5, 12.0, 'https://www.allibert-trekking.com/uploads/media/images/851-jma-montagne.jpeg', 4),
(1, 'GR 1200', 'YO SUPER.', 5, 2, 3.5, 12.0, 'https://jeromeobiols.com/wordpress/wp-content/uploads/30122016_JOB5054F_Lever_Brenva-1920.jpg', 4),
(1, 'GR 1', 'YO SUPER.', 5, 2, 3.5, 12.0, 'https://jeromeobiols.com/wordpress/wp-content/uploads/30122016_JOB5054F_Lever_Brenva-1920.jpg', 4),
(1, 'GR 2', 'YO SUPER.', 5, 2, 3.5, 12.0, 'https://jeromeobiols.com/wordpress/wp-content/uploads/30122016_JOB5054F_Lever_Brenva-1920.jpg', 4),
(2, 'Forest Path', 'A serene path through the forest.', 10, 5, 2.0, 8.0, 'forest.jpg', 5),
(3, 'Desert Walk', 'A challenging walk through the desert.', 3, 1, 4.0, 15.0, 'desert.jpg', 3),
(4, 'City Tour', 'A tour through the historic city.', 8, 3, 2.5, 10.0, 'city.jpg', 4),
(5, 'Beachside Path', 'A relaxing path along the beach.', 12, 7, 3.0, 9.0, 'beach.jpg', 5),
(6, 'River Route', 'A route following the river.', 4, 2, 5.0, 20.0, 'river.jpg', 3),
(7, 'Hill Climb', 'A steep climb up the hill.', 6, 4, 2.5, 7.0, 'hill.jpg', 4),
(8, 'Park Loop', 'A loop around the city park.', 9, 6, 1.5, 5.0, 'park.jpg', 5),
(9, 'Countryside Trek', 'A trek through the countryside.', 7, 3, 6.0, 18.0, 'countryside.jpg', 4),
(10, 'Lakeside Walk', 'A walk around the lake.', 11, 8, 2.0, 8.0, 'lake.jpg', 5),
(11, 'Sunny Meadow', 'A bright walk through meadows.', 14, 5, 4.0, 10.0, 'https://example.com/meadow.jpg', 5),
(12, 'Snowy Peak', 'A cold climb to the peak.', 7, 3, 5.0, 12.0, 'https://example.com/snowy_peak.jpg', 4),
(13, 'Cave Exploration', 'A dark and exciting cave.', 11, 6, 2.5, 8.0, 'https://example.com/cave.jpg', 5),
(14, 'Jungle Trek', 'A dense jungle trek.', 8, 4, 3.5, 9.0, 'https://example.com/jungle.jpg', 4),
(15, 'Waterfall Route', 'A route passing several waterfalls.', 15, 7, 4.5, 10.5, 'https://example.com/waterfall.jpg', 5);

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
(5, 'Cliff View', 'Cliff', 39.8292, -104.9203),
(6, 'Bridge Crossing', 'Bridge', 39.8192, -104.9103),
(6, 'Waterfall View', 'Waterfall', 39.8292, -104.9003),
(7, 'Hill Base', 'Base', 39.8392, -104.8903),
(7, 'Hill Top', 'Top', 39.8492, -104.8803),
(8, 'Park Entrance', 'Entrance', 39.8592, -104.8703),
(8, 'Park Center', 'Center', 39.8692, -104.8603),
(9, 'Countryside Start', 'Start', 39.8792, -104.8503),
(9, 'Countryside End', 'End', 39.8892, -104.8403),
(10, 'Lake Start', 'Start', 39.8992, -104.8303),
(10, 'Lake End', 'End', 39.9092, -104.8203),
(11, 'Meadow Entrance', 'Entrance', 39.9192, -104.8103),
(11, 'Meadow Center', 'Center', 39.9292, -104.8003),
(12, 'Peak Base', 'Base', 39.9392, -104.7903),
(12, 'Peak Summit', 'Summit', 39.9492, -104.7803),
(13, 'Cave Entrance', 'Entrance', 39.9592, -104.7703),
(13, 'Cave Depths', 'Depths', 39.9692, -104.7603),
(14, 'Jungle Start', 'Start', 39.9792, -104.7503),
(14, 'Jungle End', 'End', 39.9892, -104.7403),
(15, 'Waterfall Start', 'Start', 39.9992, -104.7303),
(15, 'Waterfall End', 'End', 40.0092, -104.7203),
(1, 'Additional Mark 1', 'Mark 1', 40.0192, -104.7103),
(1, 'Additional Mark 2', 'Mark 2', 40.0292, -104.7003),
(2, 'Additional Mark 3', 'Mark 3', 40.0392, -104.6903),
(2, 'Additional Mark 4', 'Mark 4', 40.0492, -104.6803),
(3, 'Additional Mark 5', 'Mark 5', 40.0592, -104.6703),
(3, 'Additional Mark 6', 'Mark 6', 40.0692, -104.6603),
(4, 'Additional Mark 7', 'Mark 7', 40.0792, -104.6503),
(4, 'Additional Mark 8', 'Mark 8', 40.0892, -104.6403),
(5, 'Additional Mark 9', 'Mark 9', 40.0992, -104.6303),
(5, 'Additional Mark 10', 'Mark 10', 40.1092, -104.6203),
(6, 'Additional Mark 11', 'Mark 11', 40.1192, -104.6103),
(6, 'Additional Mark 12', 'Mark 12', 40.1292, -104.6003),
(7, 'Additional Mark 13', 'Mark 13', 40.1392, -104.5903),
(7, 'Additional Mark 14', 'Mark 14', 40.1492, -104.5803),
(8, 'Additional Mark 15', 'Mark 15', 40.1592, -104.5703),
(8, 'Additional Mark 16', 'Mark 16', 40.1692, -104.5603),
(9, 'Additional Mark 17', 'Mark 17', 40.1792, -104.5503),
(9, 'Additional Mark 18', 'Mark 18', 40.1892, -104.5403),
(10, 'Additional Mark 19', 'Mark 19', 40.1992, -104.5303),
(10, 'Additional Mark 20', 'Mark 20', 40.2092, -104.5203);

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
(10, 1, 'Hill Route 2', 'The second hill segment.', 7.0),
(11, 12, 'Meadow Route 1', 'The first meadow segment.', 4.0),
(12, 13, 'Snowy Peak Route 1', 'The first peak segment.', 5.0),
(13, 14, 'Cave Route 1', 'The first cave segment.', 3.0),
(14, 15, 'Jungle Route 1', 'The first jungle segment.', 6.0),
(1, 21, 'Additional Route 1', 'An additional route segment.', 4.5),
(21, 22, 'Additional Route 2', 'An additional route segment.', 3.2),
(23, 24, 'Additional Route 3', 'An additional route segment.', 5.1),
(25, 26, 'Additional Route 4', 'An additional route segment.', 2.8),
(27, 28, 'Additional Route 5', 'An additional route segment.', 4.6),
(29, 30, 'Additional Route 6', 'An additional route segment.', 3.7),
(31, 32, 'Additional Route 7', 'An additional route segment.', 4.2),
(33, 34, 'Additional Route 8', 'An additional route segment.', 5.4),
(35, 36, 'Additional Route 9', 'An additional route segment.', 3.9),
(37, 38, 'Additional Route 10', 'An additional route segment.', 4.8),
(39, 40, 'Additional Route 11', 'An additional route segment.', 4.0);

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
(2, 1),
(2, 2),
(1, 4),
(3, 3),
(4, 4),
(1, 5),
(6, 6),
(7, 7),
(8, 8),
(9, 9),
(10, 10),
(11, 11),
(12, 12),
(13, 13),
(14, 14),
(15, 15),
(1, 2),
(2, 3),
(3, 4),
(4, 5),
(5, 6),
(6, 7),
(7, 8),
(8, 9),
(9, 10),
(10, 11),
(11, 12),
(12, 13),
(13, 14),
(14, 15);

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
(5, 1, 'Very nice, loved it!', '2024-07-01'),
(6, 1, 'cool, loved it!', '2024-08-01'),
(2, 2, 'Very peaceful walk.', '2024-06-02'),
(3, 3, 'A bit too hot.', '2024-06-03'),
(4, 4, 'Loved the historic sights.', '2024-06-04'),
(5, 5, 'The beach was amazing!', '2024-06-05'),
(6, 6, 'Nice river views.', '2024-06-06'),
(7, 7, 'Quite a climb.', '2024-06-07'),
(8, 8, 'The park was beautiful.', '2024-06-08'),
(9, 9, 'Peaceful countryside trek.', '2024-06-09'),
(10, 10, 'The lake walk was so relaxing.', '2024-06-10'),
(11, 11, 'Loved the sunny meadows.', '2024-06-11'),
(12, 12, 'The snowy peak was challenging.', '2024-06-12'),
(13, 13, 'The cave was spooky but exciting.', '2024-06-13'),
(14, 14, 'The jungle trek was dense and thrilling.', '2024-06-14'),
(15, 15, 'Waterfalls were breathtaking.', '2024-06-15'),
(1, 2, 'Had a great time here.', '2024-06-16'),
(2, 3, 'Nice and warm.', '2024-06-17'),
(3, 4, 'Lots of history.', '2024-06-18'),
(4, 5, 'Best beach ever.', '2024-06-19'),
(5, 6, 'The river was serene.', '2024-06-20'),
(6, 7, 'Hill climbing was fun.', '2024-06-21'),
(7, 8, 'Enjoyed the park.', '2024-06-22'),
(8, 9, 'The countryside was refreshing.', '2024-06-23'),
(9, 10, 'Loved the lake.', '2024-06-24'),
(10, 11, 'Meadow walks were bright.', '2024-06-25'),
(11, 12, 'Snow peak was a challenge.', '2024-06-26'),
(12, 13, 'Cave adventure was thrilling.', '2024-06-27'),
(13, 14, 'Jungle was an adventure.', '2024-06-28'),
(14, 15, 'The waterfalls were gorgeous.', '2024-06-29');

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
(10, 'cdefgh567890'),
(11, 'tokenvalue11'),
(12, 'tokenvalue12'),
(13, 'tokenvalue13'),
(14, 'tokenvalue14'),
(15, 'tokenvalue15');

UNLOCK TABLES;

CREATE USER 'api'@'%' IDENTIFIED BY 'azerty';
GRANT SELECT, INSERT, UPDATE, DELETE ON MountainJourney.* TO 'api'@'%';
FLUSH PRIVILEGES;
