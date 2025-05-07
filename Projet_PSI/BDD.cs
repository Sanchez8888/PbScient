using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Projet_PSI
{
    internal class BDD
    {
        string nom;
        public BDD(string nom)
        {
            this.nom = nom;
        }

        /// <summary>
        /// Récupère les données d'un client ou d'un cuisinier à partir de la base de données.
        /// </summary>
        /// <param name="IdClient">L'ID du client pour lequel récupérer les données. Utilisez 0 si l'ID du cuisinier est fourni.</param>
        /// <param name="IdCuisinier">L'ID du cuisinier pour lequel récupérer les données. Utilisez 0 si l'ID du client est fourni.</param>
        /// <returns>Une liste de chaînes contenant les données du client ou du cuisinier, y compris le nom, prénom, adresse, etc.</returns>
        public List<string> GetData(int IdClient, int IdCuisinier)
        {
            List<string> result = new List<string>();
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string table = IdClient != 0 ? "Client" : "Cuisinier";
            string idColumn = IdClient != 0 ? "Id_Client" : "Id_Cuisinier";
            int id = IdClient != 0 ? IdClient : IdCuisinier;

            string query = $@"
        SELECT nom, prenom, adresse, code_postal, ville, telephone, mail, motdepasse, radie, idMetro, nomMetro 
        FROM {table} 
        WHERE {idColumn} = @Id";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader[i]?.ToString() ?? "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Vérifie si l'adresse e-mail existe déjà dans la base de données, que ce soit pour un client ou un cuisinier.
        /// </summary>
        /// <param name="email">L'adresse e-mail à vérifier dans la base de données.</param>
        /// <returns>Retourne true si l'adresse e-mail existe dans la table Client ou Cuisinier, sinon false.</returns>
        public bool CheckMail(string email)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"SELECT COUNT(*) FROM Client WHERE mail = @Email UNION ALL SELECT COUNT(*) FROM Cuisinier WHERE mail = @Email;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32(0) > 0)
                                return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Vérifie si le mot de passe fourni correspond à celui enregistré dans la base de données pour un client donné.
        /// </summary>
        /// <param name="email">L'adresse e-mail du client à vérifier.</param>
        /// <param name="password">Le mot de passe fourni par le client.</param>
        /// <returns>Retourne l'identifiant du client (id_Client) si l'e-mail et le mot de passe correspondent ou retourne 0 si aucun client correspondant n'est trouvé ou si le mot de passe ne correspond pas.</returns>
        public int CheckPassClient(string email, string password)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string ligne = "SELECT id_Client, motdepasse FROM Client WHERE mail = @mail;";
                    MySqlCommand cmd = new MySqlCommand(ligne, conn);
                    cmd.Parameters.AddWithValue("@mail", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string motDePasseEnBase = reader["motdepasse"].ToString();
                            if (motDePasseEnBase == password)
                            {
                                return Convert.ToInt32(reader["id_Client"]);
                            }
                        }
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// Vérifie si le mot de passe fourni correspond à celui enregistré dans la base de données pour un cuisinier donné.
        /// </summary>
        /// <param name="email">L'adresse e-mail du cuisinier à vérifier.</param>
        /// <param name="password">Le mot de passe fourni par le cuisinier.</param>
        /// <returns>Retourne l'identifiant du cuisinier (id_Cuisinier) si l'e-mail et le mot de passe correspondent ou retourne 0 si aucun cuisinier correspondant n'est trouvé ou si le mot de passe ne correspond pas.</returns>
        public int CheckPassCuisinier(string email, string password)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string ligne = "SELECT id_Cuisinier, motdepasse FROM Cuisinier WHERE mail = @mail;";
                    MySqlCommand cmd = new MySqlCommand(ligne, conn);
                    cmd.Parameters.AddWithValue("@mail", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string motDePasseEnBase = reader["motdepasse"].ToString();
                            if (motDePasseEnBase == password)
                            {
                                return Convert.ToInt32(reader["id_Cuisinier"]);
                            }
                        }
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// Ajoute un nouveau client à la base de données avec les informations fournies.
        /// </summary>
        /// <param name="nom">Nom du client.</param>
        /// <param name="prenom">Prénom du client.</param>
        /// <param name="adresse">Adresse postale du client.</param>
        /// <param name="codePostal">Code postal du client.</param>
        /// <param name="ville">Ville du client.</param>
        /// <param name="telephone">Numéro de téléphone du client.</param>
        /// <param name="mail">Adresse e-mail du client.</param>
        /// <param name="motDePasse">Mot de passe du client.</param>
        /// <param name="radie">Indique si le client est radié (true) ou non (false).</param>
        /// <param name="idMetro">Identifiant de la station de métro associée.</param>
        /// <param name="nomMetro">Nom de la station de métro associée.</param>
        /// <returns>Retourne true si l'insertion a été effectuée avec succès (au moins une ligne insérée), sinon false.</returns>
        public bool AjouterClient(string nom, string prenom, string adresse, int codePostal, string ville, string telephone, string mail, string motDePasse, bool radie, int idMetro, string nomMetro)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"INSERT INTO Client (nom, prenom, adresse, code_postal, ville, telephone, mail, motdepasse, radie, idMetro, nomMetro) 
                     VALUES (@Nom, @Prenom, @Adresse, @CodePostal, @Ville, @Telephone, @Mail, @MotDePasse, @Radie, @IdMetro, @NomMetro);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@Prenom", prenom);
                    cmd.Parameters.AddWithValue("@Adresse", adresse);
                    cmd.Parameters.AddWithValue("@CodePostal", codePostal);
                    cmd.Parameters.AddWithValue("@Ville", ville);
                    cmd.Parameters.AddWithValue("@Telephone", telephone);
                    cmd.Parameters.AddWithValue("@Mail", mail);
                    cmd.Parameters.AddWithValue("@MotDePasse", motDePasse);
                    cmd.Parameters.AddWithValue("@Radie", radie);
                    cmd.Parameters.AddWithValue("@IdMetro", idMetro);
                    cmd.Parameters.AddWithValue("@NomMetro", nomMetro);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Ajoute un nouveau cuisinier à la base de données avec les informations fournies.
        /// </summary>
        /// <param name="nom">Nom du cuisinier.</param>
        /// <param name="prenom">Prénom du cuisinier.</param>
        /// <param name="adresse">Adresse postale du cuisinier.</param>
        /// <param name="codePostal">Code postal du cuisinier.</param>
        /// <param name="ville">Ville du cuisinier.</param>
        /// <param name="telephone">Numéro de téléphone du cuisinier.</param>
        /// <param name="mail">Adresse e-mail du cuisinier.</param>
        /// <param name="motDePasse">Mot de passe du cuisinier.</param>
        /// <param name="radie">Indique si le cuisinier est radié (true) ou non (false).</param>
        /// <param name="idMetro">Identifiant de la station de métro associée.</param>
        /// <param name="nomMetro">Nom de la station de métro associée.</param>
        /// <returns>Retourne true si l'insertion a réussi (au moins une ligne insérée), sinon false.</returns>
        public bool AjouterCuisinier(string nom, string prenom, string adresse, int codePostal, string ville, string telephone, string mail, string motDePasse, bool radie, int idMetro, string nomMetro)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"INSERT INTO Cuisinier (nom, prenom, adresse, code_postal, ville, telephone, mail, motdepasse, radie, idMetro, nomMetro) 
                     VALUES (@Nom, @Prenom, @Adresse, @CodePostal, @Ville, @Telephone, @Mail, @MotDePasse, @Radie, @IdMetro, @NomMetro);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@Prenom", prenom);
                    cmd.Parameters.AddWithValue("@Adresse", adresse);
                    cmd.Parameters.AddWithValue("@CodePostal", codePostal);
                    cmd.Parameters.AddWithValue("@Ville", ville);
                    cmd.Parameters.AddWithValue("@Telephone", telephone);
                    cmd.Parameters.AddWithValue("@Mail", mail);
                    cmd.Parameters.AddWithValue("@MotDePasse", motDePasse);
                    cmd.Parameters.AddWithValue("@Radie", radie);
                    cmd.Parameters.AddWithValue("@IdMetro", idMetro);
                    cmd.Parameters.AddWithValue("@NomMetro", nomMetro);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }


       /// <summary>
        /// Modifie la quantité (nombre de personnes) associée à un plat dans la base de données.
        /// </summary>
        /// <param name="idPlat">L'identifiant du plat dont la quantité doit être modifiée.</param>
        /// <param name="nouvelleQuantite">La nouvelle quantité (nombre de personnes) à associer au plat.</param>
        /// <returns>Retourne true si la mise à jour a réussi (au moins une ligne modifiée), sinon false.</returns>
        public bool ModifierQuantitePlat(int idPlat, int nouvelleQuantite)
        {
            Console.Write("essai modification");
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"UPDATE Plat SET nb_personnes = @NouvelleQuantite WHERE id_plat = @IdPlat;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@NouvelleQuantite", nouvelleQuantite);
                    cmd.Parameters.AddWithValue("@IdPlat", idPlat);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0) Console.WriteLine("Modif");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Ajoute une nouvelle commande dans la base de données.
        /// </summary>
        /// <param name="idClient">Identifiant du client ayant passé la commande.</param>
        /// <param name="idPlat">Identifiant du plat commandé.</param>
        /// <param name="idCuisinier">Identifiant du cuisinier responsable de la commande.</param>
        /// <param name="prix">Prix total de la commande.</param>
        /// <param name="dateCommande">Date de la commande au format chaîne de caractères (par exemple, "2025-05-07").</param>
        /// <returns>Retourne true si l'insertion a réussi (au moins une ligne ajoutée), sinon false.</returns>
        public bool AjouterCommande(int idClient, int idPlat, int idCuisinier, double prix, string dateCommande)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";
            string query = @"INSERT INTO Commandes (id_client, id_plat, id_cuisinier, prix, date_commande, livre) 
                     VALUES (@IdClient, @IdPlat, @IdCuisinier, @Prix, @DateCommande, @Livre);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@IdClient", idClient);
                    cmd.Parameters.AddWithValue("@IdPlat", idPlat);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);
                    cmd.Parameters.AddWithValue("@Prix", prix);
                    cmd.Parameters.AddWithValue("@DateCommande", dateCommande);
                    cmd.Parameters.AddWithValue("@Livre", false);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Ajoute un commentaire (note) d’un client pour un cuisinier dans la base de données.
        /// </summary>
        /// <param name="idClient">Identifiant du client auteur du commentaire.</param>
        /// <param name="idCuisinier">Identifiant du cuisinier concerné par le commentaire.</param>
        /// <param name="commentaire">Texte du commentaire déposé par le client.</param>
        /// <param name="dateEchange">Date de l’échange ou de l’interaction entre le client et le cuisinier.</param>
        /// <returns>Retourne true si l’ajout du commentaire a réussi (au moins une ligne insérée), sinon false.</returns>
        public bool AjouterNote(int idClient, int idCuisinier, string commentaire, DateTime dateEchange)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"INSERT INTO Note (id_client, id_cuisinier, commente, date_d_echange) VALUES (@IdClient, @IdCuisinier, @Commentaire, @DateEchange);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@IdClient", idClient);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);
                    cmd.Parameters.AddWithValue("@Commentaire", commentaire);
                    cmd.Parameters.AddWithValue("@DateEchange", dateEchange.ToString("yyyy-MM-dd"));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Ajoute une nouvelle livraison dans la base de données.
        /// </summary>
        /// <param name="idCuisinier">Identifiant du cuisinier en charge de la livraison.</param>
        /// <param name="idCommande">Identifiant de la commande livrée.</param>
        /// <param name="dateLivraison">Date de la livraison, au format "yyyy-MM-dd".</param>
        /// <param name="adresseLivraison">Adresse à laquelle la commande doit être livrée.</param>
        /// <param name="trajet">Description ou données relatives au trajet effectué.</param>
        /// <returns>Retourne true si l’insertion de la livraison a réussi (au moins une ligne insérée), sinon false.</returns>
        public bool AjouterLivraison(int idCuisinier, int idCommande, string dateLivraison, string adresseLivraison, string trajet)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"INSERT INTO Livraison (id_cuisinier, id_commande, date_livraison, adresse_livraison, trajet) VALUES (@IdCuisinier, @IdCommande, @DateLivraison, @AdresseLivraison, @Trajet);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);
                    cmd.Parameters.AddWithValue("@IdCommande", idCommande);
                    cmd.Parameters.AddWithValue("@DateLivraison", dateLivraison);
                    cmd.Parameters.AddWithValue("@AdresseLivraison", adresseLivraison);
                    cmd.Parameters.AddWithValue("@Trajet", trajet);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Récupère les données de tous les plats enregistrés dans la base de données.
        /// </summary>
        /// <returns>Une liste de listes de chaînes représentant les lignes de la table Plat.</returns>
        public List<List<string>> GetDataPlats()
        {
            List<List<string>> result = new List<List<string>>();
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};CharSet=utf8;";

            string query = @"SELECT id_plat, nom_plat, id_cuisinier, nb_personnes, type_plat, prix, date_fabrication, date_expiration, regime, origine FROM Plat";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            List<string> platData = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                platData.Add(reader[i]?.ToString() ?? "NULL");
                            }
                            result.Add(platData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            return result;
        }
        
        /// <summary>
        /// Récupère toutes les commandes enregistrées dans la base de données.
        /// </summary>
        /// <returns>Une liste de listes de chaînes représentant les lignes de la table Commandes.</returns>
        public List<List<string>> GetDataCommandes()
        {
            List<List<string>> result = new List<List<string>>();
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};CharSet=utf8;";

            string query = @"SELECT id_commande, id_client, id_plat, id_cuisinier, prix, date_commande, livre FROM Commandes";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            List<string> commandeData = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                commandeData.Add(reader[i]?.ToString() ?? "NULL");
                            }
                            result.Add(commandeData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Récupère les informations personnelles d'un cuisinier à partir de son identifiant.
        /// </summary>
        /// <param name="idCuisinier">L'identifiant du cuisinier.</param>
        /// <returns>Une liste de chaînes contenant les informations suivantes : prénom, nom, adresse complète (adresse + code postal + ville) ou retourne null si le cuisinier n'est pas trouvé ou si une erreur survient.</returns>
        public List<string> GetCuisinierInfo(int idCuisinier)
        {
            List<string> result = new List<string>();
            using (MySqlConnection conn = new MySqlConnection($"server=localhost;port=3306;user id=root;password=root;database={this.nom};CharSet=utf8;"))
            {
                conn.Open();
                string query = "SELECT prenom, nom, adresse, code_postal, ville FROM Cuisinier WHERE id_cuisinier = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idCuisinier);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Add(reader.GetString("prenom"));
                            result.Add(reader.GetString("nom"));
                            string adresseComplete = $"{reader.GetString("adresse")}, {reader.GetInt32("code_postal")} {reader.GetString("ville")}";
                            result.Add(adresseComplete);
                        }
                    }
                }
            }
            return result.Count == 3 ? result : null;
        }

        /// <summary>
        /// Récupère l'identifiant de la station de métro associée à un client ou un cuisinier.
        /// </summary>
        /// <param name="idClient">L'identifiant du client. Doit être différent de 0 si l'on souhaite chercher un client.</param>
        /// <param name="idCuisinier">L'identifiant du cuisinier. Utilisé si <paramref name="idClient"/> est égal à 0.</param>
        /// <returns>L'identifiant de la station de métro (idMetro) si trouvé ; -1 en cas d'erreur ou si aucun résultat n'est trouvé.</returns>
        public int GetMetroId(int idClient, int idCuisinier)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";
            string table = idClient != 0 ? "Client" : "Cuisinier";
            string idColumn = idClient != 0 ? "id_client" : "id_cuisinier";
            int id = idClient != 0 ? idClient : idCuisinier;

            string query = $@"SELECT idMetro FROM {table} WHERE {idColumn} = @Id";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int idMetro))
                    {
                        return idMetro;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            return -1;
        }

        /// <summary>
        /// Met à jour les informations d'un client dans la base de données.
        /// </summary>
        /// <param name="idClient">L'identifiant du client à mettre à jour.</param>
        /// <param name="nom">Le nom du client.</param>
        /// <param name="prenom">Le prénom du client.</param>
        /// <param name="adresse">L'adresse du client.</param>
        /// <param name="codePostal">Le code postal de l'adresse du client.</param>
        /// <param name="ville">La ville de l'adresse du client.</param>
        /// <param name="telephone">Le numéro de téléphone du client.</param>
        /// <param name="mail">L'adresse mail du client.</param>
        /// <param name="motDePasse">Le mot de passe du client.</param>
        /// <param name="idMetro">L'identifiant de la station de métro associée au client.</param>
        /// <param name="nomMetro">Le nom de la station de métro associée au client.</param>
        /// <returns>true si les informations ont été mises à jour avec succès, <c>false</c> en cas d'échec.</returns>
        public bool MettreAJourClient(int idClient, string nom, string prenom, string adresse, int codePostal, string ville, string telephone, string mail, string motDePasse, int idMetro, string nomMetro)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"
        UPDATE Client SET
            nom = @Nom,
            prenom = @Prenom,
            adresse = @Adresse,
            code_postal = @CodePostal,
            ville = @Ville,
            telephone = @Telephone,
            mail = @Mail,
            motdepasse = @MotDePasse,
            idMetro = @IdMetro,
            nomMetro = @NomMetro
        WHERE id_client = @IdClient;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@Prenom", prenom);
                    cmd.Parameters.AddWithValue("@Adresse", adresse);
                    cmd.Parameters.AddWithValue("@CodePostal", codePostal);
                    cmd.Parameters.AddWithValue("@Ville", ville);
                    cmd.Parameters.AddWithValue("@Telephone", telephone);
                    cmd.Parameters.AddWithValue("@Mail", mail);
                    cmd.Parameters.AddWithValue("@MotDePasse", motDePasse);
                    cmd.Parameters.AddWithValue("@IdMetro", idMetro);
                    cmd.Parameters.AddWithValue("@NomMetro", nomMetro);
                    cmd.Parameters.AddWithValue("@IdClient", idClient);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur MettreAJourClient : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Marque un client comme radié (supprimé) dans la base de données, sans supprimer ses données.
        /// </summary>
        /// <param name="idClient">L'identifiant du client à radier.</param>
        /// <returns>true si le client a été radié avec succès, false en cas d'échec.</returns>
        public bool SupprimerClient(int idClient)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"UPDATE Client SET radie = TRUE WHERE id_client = @IdClient;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdClient", idClient);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur SupprimerClient : " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Met à jour les informations d'un cuisinier dans la base de données.
        /// </summary>
        /// <param name="idCuisinier">L'identifiant du cuisinier à mettre à jour.</param>
        /// <param name="nom">Le nom du cuisinier.</param>
        /// <param name="prenom">Le prénom du cuisinier.</param>
        /// <param name="adresse">L'adresse du cuisinier.</param>
        /// <param name="codePostal">Le code postal du cuisinier.</param>
        /// <param name="ville">La ville du cuisinier.</param>
        /// <param name="telephone">Le numéro de téléphone du cuisinier.</param>
        /// <param name="mail">L'adresse e-mail du cuisinier.</param>
        /// <param name="motDePasse">Le mot de passe du cuisinier.</param>
        /// <param name="idMetro">L'identifiant du métro du cuisinier.</param>
        /// <param name="nomMetro">Le nom du métro du cuisinier.</param>
        /// <returns>true si les informations du cuisinier ont été mises à jour avec succès, false en cas d'échec.</returns>
        public bool MettreAJourCuisinier(int idCuisinier, string nom, string prenom, string adresse, int codePostal, string ville, string telephone, string mail, string motDePasse, int idMetro, string nomMetro)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"
UPDATE Cuisinier SET
    nom = @Nom,
    prenom = @Prenom,
    adresse = @Adresse,
    code_postal = @CodePostal,
    ville = @Ville,
    telephone = @Telephone,
    mail = @Mail,
    motdepasse = @MotDePasse,
    idMetro = @IdMetro,
    nomMetro = @NomMetro
WHERE id_cuisinier = @IdCuisinier;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@Prenom", prenom);
                    cmd.Parameters.AddWithValue("@Adresse", adresse);
                    cmd.Parameters.AddWithValue("@CodePostal", codePostal);
                    cmd.Parameters.AddWithValue("@Ville", ville);
                    cmd.Parameters.AddWithValue("@Telephone", telephone);
                    cmd.Parameters.AddWithValue("@Mail", mail);
                    cmd.Parameters.AddWithValue("@MotDePasse", motDePasse);
                    cmd.Parameters.AddWithValue("@IdMetro", idMetro);
                    cmd.Parameters.AddWithValue("@NomMetro", nomMetro);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur MettreAJourCuisinier : " + ex.Message);
                return false;
            }
        }

        public bool SupprimerCuisinier(int idCuisinier)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = @"UPDATE Cuisinier SET radie = TRUE WHERE id_cuisinier = @IdCuisinier;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdCuisinier", idCuisinier);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur SupprimerCuisinier : " + ex.Message);
                return false;
            }
        }
        public bool ValiderCommande(int idCommande)
        {
            string connectionString = $"server=localhost;port=3306;user id=root;password=root;database={this.nom};";

            string query = "UPDATE Commandes SET livre = TRUE WHERE id_commande = @IdCommande;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdCommande", idCommande);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur ValiderCommande : " + ex.Message);
                return false;
            }
        }
        public bool AjouterPlat(string nom, int idCuisinier, int nbPers, string type, double prix, string dateFab, string dateExp, string regime, string origine)
        {
            string query = "INSERT INTO Plat (nom_plat, id_cuisinier, nb_personnes, type_plat, prix, date_fabrication, date_expiration, regime, origine) " +
                           "VALUES (@nom, @idCuisinier, @nb, @type, @prix, @fab, @exp, @regime, @origine)";
            try
            {
                using (MySqlConnection conn = new MySqlConnection($"server=localhost;port=3306;user id=root;password=root;database={this.nom};"))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    cmd.Parameters.AddWithValue("@nb", nbPers);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@prix", prix);
                    cmd.Parameters.AddWithValue("@fab", dateFab);
                    cmd.Parameters.AddWithValue("@exp", dateExp);
                    cmd.Parameters.AddWithValue("@regime", regime);
                    cmd.Parameters.AddWithValue("@origine", origine);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur ajout plat : " + ex.Message);
                return false;
            }
        }





    }
}
