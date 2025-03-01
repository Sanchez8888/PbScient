DROP DATABASE IF EXISTS site;
CREATE DATABASE site;
USE site;

DROP TABLE IF EXISTS Cuisinier;
CREATE TABLE IF NOT EXISTS Cuisinier(
	id_cuisinier VARCHAR(50) PRIMARY KEY,
    nom VARCHAR(50),
    prenom VARCHAR(50),
    adresse VARCHAR(50),
    téléphone INTEGER,
    mail VARCHAR(50),
    radié BOOL
);
DROP TABLE IF EXISTS Plat;
CREATE TABLE IF NOT EXISTS Plat(
	nom_plat VARCHAR(50) PRIMARY KEY,
    id_cuisinier VARCHAR(50),
    nb_personnes INTEGER,
    type_plat VARCHAR(50),
    prix DOUBLE,
    date_fabrication DATE,
    date_expiration DATE,
    origine VARCHAR(50),
    ingrédients VARCHAR(50)
);
DROP TABLE IF EXISTS Client;
CREATE TABLE IF NOT EXISTS Client(
	id_client VARCHAR(50) PRIMARY KEY,
    nom VARCHAR(50),
    prenom VARCHAR(50),
    adresse VARCHAR(50),
    téléphone VARCHAR(10),
    mail VARCHAR(100),
    motdepasse VARCHAR(50),
    radié BOOL
);
DROP TABLE IF EXISTS Historique;
CREATE TABLE IF NOT EXISTS Historique(
	id_historique VARCHAR(50) PRIMARY KEY,
    historique_commande VARCHAR(50),
    historique_livraison VARCHAR(50)
);    
DROP TABLE IF EXISTS Commandes;
CREATE TABLE IF NOT EXISTS Commandes(
	id_commande VARCHAR(50) PRIMARY KEY,
    id_clients VARCHAR(50),
    nom_plat VARCHAR(50),
    prix DOUBLE,
    date_commande VARCHAR(50),
    id_historique VARCHAR(50) NOT NULL,
    UNIQUE(id_historique),
    FOREIGN KEY(id_historique) REFERENCES HISTORIQUE(id_historique)
);

DROP TABLE IF EXISTS Livraisons;
CREATE TABLE IF NOT EXISTS Livraisons(
	id_livraison VARCHAR(50) PRIMARY KEY,
    date_livraison DATE,
    adresse_livraison VARCHAR(50),
    trajet VARCHAR(50),
    distance DOUBLE,
    temps INTEGER,
    id_client VARCHAR(50) NOT NULL,
    id_commande VARCHAR(50) NOT NULL,
    id_historique VARCHAR(50) NOT NULL,
    UNIQUE(id_historique),
    FOREIGN KEY(id_client) REFERENCES CLIENT(id_client),
    FOREIGN KEY(id_commande) REFERENCES COMMANDES(id_commande),
    FOREIGN KEY(id_historique) REFERENCES HISTORIQUE(id_historique)
);
DROP TABLE IF EXISTS prépare;
CREATE TABLE IF NOT EXISTS prépare(
   nom_plat VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(nom_plat, id_cuisinier),
   FOREIGN KEY(nom_plat) REFERENCES Plat(nom_plat),
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_cuisinier)
);
DROP TABLE IF EXISTS passe;
CREATE TABLE IF NOT EXISTS passe(
   id_client VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(id_client, id_commande),
   FOREIGN KEY(id_client) REFERENCES Client(id_client),
   FOREIGN KEY(id_commande) REFERENCES Commandes(id_commande)
);
DROP TABLE IF EXISTS comporte;
CREATE TABLE IF NOT EXISTS comporte(
   nom_plat VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(nom_plat, id_commande),
   FOREIGN KEY(nom_plat) REFERENCES Plat(nom_plat),
   FOREIGN KEY(id_commande) REFERENCES Commandes(id_commande)
);
DROP TABLE IF EXISTS note;
CREATE TABLE IF NOT EXISTS note(
   id_client VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(id_client, id_cuisinier),
   FOREIGN KEY(id_client) REFERENCES Client(id_client),
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_cuisinier)
);

    
