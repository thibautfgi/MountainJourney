DROP TABLE IF EXISTS 'Utilisateur';
CREATE TABLE Utilisateur (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(255) NOT NULL,
    date_inscription TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS 'Parcours';
CREATE TABLE Parcours (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titre VARCHAR(255) NOT NULL,
    description TEXT,
    date_creation TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    utilisateur_id INT,
    latitude_depart DECIMAL(10, 8),
    longitude_depart DECIMAL(11, 8),
    distance DECIMAL(5, 2),
    duree_estime TIME,
    FOREIGN KEY (utilisateur_id) REFERENCES Utilisateur(id) ON DELETE CASCADE
);


DROP TABLE IF EXISTS 'Commentaire';
CREATE TABLE Commentaire (
    id INT AUTO_INCREMENT PRIMARY KEY,
    contenu TEXT NOT NULL,
    date_creation TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    utilisateur_id INT,
    parcours_id INT,
    FOREIGN KEY (utilisateur_id) REFERENCES Utilisateur(id) ON DELETE CASCADE,
    FOREIGN KEY (parcours_id) REFERENCES Parcours(id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS 'Favori';
CREATE TABLE Favori (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utilisateur_id INT,
    parcours_id INT,
    date_ajout TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (utilisateur_id) REFERENCES Utilisateur(id) ON DELETE CASCADE,
    FOREIGN KEY (parcours_id) REFERENCES Parcours(id) ON DELETE CASCADE
);


CREATE USER 'api_user'@'%' IDENTIFIED BY 'Azerty123';
GRANT SELECT, INSERT, UPDATE, DELETE ON MountainJourney.* TO 'api_user'@'%';
FLUSH PRIVILEGES;