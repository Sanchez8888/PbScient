# PbScient
CREATE TABLE Client(
   id_client VARCHAR(50),
   mail VARCHAR(50) NOT NULL,
   nom VARCHAR(50),
   prenom VARCHAR(50),
   adresse VARCHAR(50),
   motdepasse VARCHAR(50),
   radié_ LOGICAL,
   PRIMARY KEY(id_client)
);

CREATE TABLE Plat(
   nom_plat VARCHAR(50),
   nb_personnes INT,
   origine VARCHAR(50),
   type_plat VARCHAR(50),
   prix DOUBLE,
   date_fabrication VARCHAR(50),
   date_expiration_ VARCHAR(50),
   ingrédients VARCHAR(50),
   PRIMARY KEY(nom_plat)
);

CREATE TABLE Cuisinier(
   id_cuisinier VARCHAR(50),
   nom VARCHAR(50),
   prenom VARCHAR(50),
   adresse VARCHAR(50),
   téléphone INT,
   mail VARCHAR(50),
   radié_ LOGICAL,
   PRIMARY KEY(id_cuisinier)
);

CREATE TABLE Historique(
   id_historique VARCHAR(50),
   historique_commande VARCHAR(50),
   historique_livraison VARCHAR(50),
   PRIMARY KEY(id_historique)
);

CREATE TABLE Commandes(
   id_commande VARCHAR(50),
   id_client VARCHAR(50),
   nom_plat VARCHAR(50),
   prix VARCHAR(50),
   date_commande VARCHAR(50),
   id_historique VARCHAR(50) NOT NULL,
   PRIMARY KEY(id_commande),
   UNIQUE(id_historique),
   FOREIGN KEY(id_historique) REFERENCES Historique(id_historique)
);

CREATE TABLE Livraisons(
   id_livraison VARCHAR(50),
   date_livraison VARCHAR(50),
   adresse_livraison VARCHAR(50),
   trajet VARCHAR(50),
   distance VARCHAR(50),
   temps VARCHAR(50),
   id_client VARCHAR(50) NOT NULL,
   id_commande VARCHAR(50) NOT NULL,
   id_historique VARCHAR(50) NOT NULL,
   PRIMARY KEY(id_livraison),
   UNIQUE(id_historique),
   FOREIGN KEY(id_client) REFERENCES Client(id_client),
   FOREIGN KEY(id_commande) REFERENCES Commandes(id_commande),
   FOREIGN KEY(id_historique) REFERENCES Historique(id_historique)
);

CREATE TABLE prépare(
   nom_plat VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(nom_plat, id_cuisinier),
   FOREIGN KEY(nom_plat) REFERENCES Plat(nom_plat),
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_cuisinier)
);

CREATE TABLE passe(
   id_client VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(id_client, id_commande),
   FOREIGN KEY(id_client) REFERENCES Client(id_client),
   FOREIGN KEY(id_commande) REFERENCES Commandes(id_commande)
);

CREATE TABLE comporte(
   nom_plat VARCHAR(50),
   id_commande VARCHAR(50),
   PRIMARY KEY(nom_plat, id_commande),
   FOREIGN KEY(nom_plat) REFERENCES Plat(nom_plat),
   FOREIGN KEY(id_commande) REFERENCES Commandes(id_commande)
);

CREATE TABLE note(
   id_client VARCHAR(50),
   id_cuisinier VARCHAR(50),
   PRIMARY KEY(id_client, id_cuisinier),
   FOREIGN KEY(id_client) REFERENCES Client(id_client),
   FOREIGN KEY(id_cuisinier) REFERENCES Cuisinier(id_cuisinier)
);
