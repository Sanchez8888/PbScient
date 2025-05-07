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
