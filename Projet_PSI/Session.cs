    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    namespace Projet_PSI
    {
        internal class Session
        {
            int IdClient;
            int IdCuisinier;
            string DB;
            BDD BDD;
            List<string> Data;
            List<List<string>> DataPlats;
            List<List<string>> DataCommandes;
            Dictionary<int, int> PanierClient;
            private Graphe graphe;

        private Connection connection;

        public Session(int IdClient, int IdCuisinier, string DB, Graphe graphe, Connection connection)
        {
            this.IdClient = IdClient;
            this.IdCuisinier = IdCuisinier;
            this.DB = DB;
            this.BDD = new BDD(DB);
            this.Data = BDD.GetData(IdClient, IdCuisinier);
            this.DataPlats = BDD.GetDataPlats();
            this.DataCommandes = BDD.GetDataCommandes();
            this.PanierClient = new Dictionary<int, int>();
            this.graphe = graphe;
            this.connection = connection;
        }



        public void OpenSession()
            {
                if (Data.Count == 0)
                {
                    MessageBox.Show("Aucune donnée trouvée pour cette session.");
                    return;
                }

                if (Data.Count >= 9 && Data[8].ToLower() == "true")
                {
                    MessageBox.Show("Ce compte a été radié.", "Compte inactif", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (IdClient != 0 && IdCuisinier != 0)
                {
                    ChoixR choix = new ChoixR(Data[0], Data[1]);
                    choix.ShowDialog();

                    if (choix.ChosenRole == "Client")
                    {
                        OpenSessionClient();
                        return;
                    }
                    else if (choix.ChosenRole == "Cuisinier")
                    {
                        OpenSessionCuisinier();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Aucun rôle sélectionné.");
                        return;
                    }
                }

                if (IdClient != 0)
                {
                    OpenSessionClient();
                }
                else if (IdCuisinier != 0)
                {
                    OpenSessionCuisinier();
                }
            }

        private void OpenSessionClient()
        {
            FormClient formClient = new FormClient(Data, DataPlats, PanierClient, BDD, IdClient, graphe, connection);
            formClient.ShowDialog();
        }


        private void OpenSessionCuisinier()
        {
            FormCuisinier form = new FormCuisinier(Data, BDD, IdCuisinier, graphe, connection);
            form.ShowDialog();
        }


        private class FormCuisinier : Form
        {
            private List<string> Data;
            private BDD BDD;
            private int idCuisinier;
            private Graphe graphe;
            private Connection connection;

            public FormCuisinier(List<string> data, BDD bdd, int idCuisinier, Graphe graphe, Connection connection)
            {
                this.Data = data;
                this.BDD = bdd;
                this.idCuisinier = idCuisinier;
                this.graphe = graphe;
                this.connection = connection; // Sauvegarde la référence de Connection
                InitUI();
            }


            private void InitUI()
            {
                this.Text = "Espace Cuisinier";
                this.Size = new System.Drawing.Size(500, 400);

                Label welcomeLabel = new Label
                {
                    Text = $"Bienvenue {Data[0]} {Data[1]} à Liv'In Paris",
                    AutoSize = true,
                    Location = new System.Drawing.Point(30, 20)
                };
                this.Controls.Add(welcomeLabel);

                Button btnAjouterPlat = new Button
                {
                    Text = "Ajouter un plat",
                    Location = new System.Drawing.Point(30, 60),
                    Size = new System.Drawing.Size(300, 30)
                };
                btnAjouterPlat.Click += (s, e) =>
                {
                    Form ajouterPlat = new Form
                    {
                        Text = "Nouveau Plat",
                        Size = new System.Drawing.Size(450, 500),
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    Label[] labels = {
            new Label { Text = "Nom du plat", Top = 30 },
            new Label { Text = "Nb personnes", Top = 70 },
            new Label { Text = "Prix (€)", Top = 110 },
            new Label { Text = "Expiration dans (jours)", Top = 150 },
            new Label { Text = "Régime", Top = 230 },
            new Label { Text = "Origine", Top = 270 }
        };

                    TextBox txtNom = new TextBox { Top = 30, Left = 180, Width = 200 };
                    TextBox txtNb = new TextBox { Top = 70, Left = 180, Width = 200 };
                    TextBox txtPrix = new TextBox { Top = 110, Left = 180, Width = 200 };
                    TextBox txtJoursExp = new TextBox { Top = 150, Left = 180, Width = 200 };
                    TextBox txtRegime = new TextBox { Top = 230, Left = 180, Width = 200 };
                    TextBox txtOrigine = new TextBox { Top = 270, Left = 180, Width = 200 };

                    CheckBox cbEntree = new CheckBox { Text = "Entrée", Top = 190, Left = 40 };
                    CheckBox cbPlat = new CheckBox { Text = "Plat", Top = 190, Left = 140 };
                    CheckBox cbDessert = new CheckBox { Text = "Dessert", Top = 190, Left = 240 };

                    foreach (var lbl in labels)
                    {
                        lbl.Left = 20;
                        lbl.Width = 150;
                        ajouterPlat.Controls.Add(lbl);
                    }

                    ajouterPlat.Controls.AddRange(new Control[] { txtNom, txtNb, txtPrix, txtJoursExp, txtRegime, txtOrigine, cbEntree, cbPlat, cbDessert });

                    Button btnAjouter = new Button
                    {
                        Text = "Ajouter le plat",
                        Left = 130,
                        Top = 320,
                        Width = 180
                    };

                    btnAjouter.Click += (sender2, e2) =>
                    {
                        if (!int.TryParse(txtNb.Text, out int nbPers) || !double.TryParse(txtPrix.Text, out double prix) || !int.TryParse(txtJoursExp.Text, out int jours))
                        {
                            MessageBox.Show("Veuillez remplir correctement les champs numériques.");
                            return;
                        }

                        string type = "";
                        if (cbEntree.Checked) type += "Entrée ";
                        if (cbPlat.Checked) type += "Plat ";
                        if (cbDessert.Checked) type += "Dessert ";
                        type = type.Trim();

                        if (string.IsNullOrEmpty(type))
                        {
                            MessageBox.Show("Veuillez sélectionner au moins un type (Entrée / Plat / Dessert).");
                            return;
                        }

                        string dateFab = DateTime.Now.ToString("yyyy-MM-dd");
                        string dateExp = DateTime.Now.AddDays(jours).ToString("yyyy-MM-dd");

                        bool success = BDD.AjouterPlat(
                            txtNom.Text, idCuisinier, nbPers,
                            type, prix,
                            dateFab, dateExp,
                            txtRegime.Text, txtOrigine.Text
                        );

                        if (success)
                            MessageBox.Show("Plat ajouté !");
                        else
                            MessageBox.Show("Erreur lors de l'ajout.");

                        ajouterPlat.Close();
                    };

                    ajouterPlat.Controls.Add(btnAjouter);
                    ajouterPlat.ShowDialog();
                };
                this.Controls.Add(btnAjouterPlat);

                Button btnCommandes = new Button
                {
                    Text = "Voir les commandes reçues",
                    Location = new System.Drawing.Point(30, 100),
                    Size = new System.Drawing.Size(300, 30)
                };
                btnCommandes.Click += (s, e) =>
                {
                    List<List<string>> commandes = BDD.GetDataCommandes();
                    List<List<string>> commandesCuisinier = commandes.FindAll(cmd => int.Parse(cmd[3]) == idCuisinier);
                    if (commandesCuisinier.Count == 0)
                        MessageBox.Show("Aucune commande reçue.");
                    else
                        new FormCommandesCuisinier(commandesCuisinier, BDD, graphe).ShowDialog();
                };
                this.Controls.Add(btnCommandes);

                Button btnParametres = new Button
                {
                    Text = "Paramètres du compte",
                    Location = new System.Drawing.Point(30, 140),
                    Size = new System.Drawing.Size(300, 30)
                };
                btnParametres.Click += BtnParametres_Click;
                this.Controls.Add(btnParametres);

                // Nouveau bouton de déconnexion
                Button btnDeconnexion = new Button
                {
                    Text = "Déconnexion",
                    Location = new System.Drawing.Point(30, 180),
                    Size = new System.Drawing.Size(300, 30)
                };
                btnDeconnexion.Click += (s, e) =>
                {
                    this.Close(); // Fermer la fenêtre actuelle du cuisinier

                    // Ouvrir à nouveau la fenêtre de connexion
                    System.Threading.Thread t = new System.Threading.Thread(() =>
                    {
                        connection.OuvrirIU();
                    });
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                };
                this.Controls.Add(btnDeconnexion);
            }




            private void BtnParametres_Click(object sender, EventArgs e)
            {
                Form form = new Form
                {
                    Text = "Paramètres du compte",
                    Size = new System.Drawing.Size(400, 500),
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label[] labels = new Label[8];
                TextBox[] textboxes = new TextBox[8];
                string[] noms = { "Nom", "Prénom", "Adresse", "Code Postal", "Ville", "Téléphone", "Email", "Mot de passe" };

                for (int i = 0; i < noms.Length; i++)
                {
                    labels[i] = new Label { Text = noms[i], Left = 20, Top = 30 + i * 35, Width = 100 };
                    textboxes[i] = new TextBox { Left = 130, Top = 30 + i * 35, Width = 200, Text = Data[i] };
                    form.Controls.Add(labels[i]);
                    form.Controls.Add(textboxes[i]);
                }

                // Changement de station
                Button btnChangerStation = new Button { Text = "Changer station", Left = 20, Top = 330, Width = 310 };
                int selectedIdMetro = int.Parse(Data[9]);
                string selectedNomMetro = Data[10];
                btnChangerStation.Click += (s, ev) =>
                {
                    var result = graphe.SelectStation();
                    if (result != null && result.Count == 2)
                    {
                        selectedIdMetro = int.Parse(result[0]);
                        selectedNomMetro = result[1];
                        MessageBox.Show($"Nouvelle station : {selectedNomMetro} (ID {selectedIdMetro})");
                    }
                };
                form.Controls.Add(btnChangerStation);

                // Mettre à jour
                Button btnMaj = new Button { Text = "Mettre à jour", Left = 20, Top = 370, Width = 150 };
                btnMaj.Click += (s, ev) =>
                {
                    if (!int.TryParse(textboxes[3].Text.Trim(), out int cp))
                    {
                        MessageBox.Show("Code postal invalide");
                        return;
                    }

                    bool ok = BDD.MettreAJourCuisinier(
                        idCuisinier,
                        textboxes[0].Text, textboxes[1].Text, textboxes[2].Text,
                        cp, textboxes[4].Text, textboxes[5].Text,
                        textboxes[6].Text, textboxes[7].Text,
                        selectedIdMetro, selectedNomMetro
                    );

                    if (ok) MessageBox.Show("Mise à jour réussie");
                    else MessageBox.Show("Erreur lors de la mise à jour");
                };
                form.Controls.Add(btnMaj);

                // Supprimer compte
                Button btnSupprimer = new Button { Text = "Supprimer compte", Left = 180, Top = 370, Width = 150 };
                btnSupprimer.Click += (s, ev) =>
                {
                    var confirm = MessageBox.Show("Voulez-vous vraiment supprimer votre compte ?", "Confirmation", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        if (BDD.SupprimerCuisinier(idCuisinier))
                        {
                            MessageBox.Show("Compte supprimé.");
                            form.Close();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression.");
                        }
                    }
                };
                form.Controls.Add(btnSupprimer);

                form.ShowDialog();
            }
        }

        public class FormCommandesCuisinier : Form
        {
            private BDD bdd;
            private Graphe graphe;
            private List<List<string>> commandes;
            private DataGridView dgv;

            public FormCommandesCuisinier(List<List<string>> commandes, BDD bdd, Graphe graphe)
            {
                this.commandes = commandes;
                this.bdd = bdd;
                this.graphe = graphe;

                this.Text = "Commandes Reçues (Cuisinier)";
                this.Size = new System.Drawing.Size(1100, 500);
                InitUI();
            }

            private void InitUI()
            {
                dgv = new DataGridView
                {
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(1040, 380),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                dgv.Columns.Add("IdCommande", "ID Commande");
                dgv.Columns.Add("IdClient", "ID Client");
                dgv.Columns.Add("IdPlat", "ID Plat");
                dgv.Columns.Add("DateCommande", "Date de commande");
                dgv.Columns.Add("Prix", "Prix (€)");
                dgv.Columns.Add("Livre", "Livré ?");

                DataGridViewButtonColumn btnTrajet = new DataGridViewButtonColumn
                {
                    HeaderText = "Trajet",
                    Text = "Trouver Trajet",
                    UseColumnTextForButtonValue = true
                };
                dgv.Columns.Add(btnTrajet);

                DataGridViewButtonColumn btnValider = new DataGridViewButtonColumn
                {
                    HeaderText = "Valider",
                    Text = "Valider Commande",
                    UseColumnTextForButtonValue = true
                };
                dgv.Columns.Add(btnValider);

                foreach (var cmd in commandes)
                {
                    string idCommande = cmd[0];
                    string idClient = cmd[1];
                    string idPlat = cmd[2];
                    string date = cmd[5];
                    string prix = cmd[4];
                    string livre = cmd.Count > 6 ? cmd[6] : "false";

                    dgv.Rows.Add(idCommande, idClient, idPlat, date, prix, livre);
                }

                dgv.CellContentClick += Dgv_CellContentClick;
                this.Controls.Add(dgv);
            }

            private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;

                var row = dgv.Rows[e.RowIndex];
                int idCommande = int.Parse(row.Cells[0].Value.ToString());
                int idClient = int.Parse(row.Cells[1].Value.ToString());

                if (e.ColumnIndex == 6) // Trajet
                {
                    try
                    {
                        int idMetroClient = bdd.GetMetroId(idClient, 0);
                        int idMetroCuisinier = bdd.GetMetroId(0, int.Parse(commandes[e.RowIndex][3]));

                        if (idMetroClient == -1 || idMetroCuisinier == -1)
                        {
                            MessageBox.Show("Station métro non trouvée.");
                            return;
                        }

                        var trajet = graphe.TrouverTrajet(idMetroClient, idMetroCuisinier);
                        MessageBox.Show(string.Join(" ➔ ", trajet), "Trajet");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur trajet: " + ex.Message);
                    }
                }
                else if (e.ColumnIndex == 7) // Valider commande
                {
                    bool success = bdd.ValiderCommande(idCommande);
                    if (success)
                    {
                        row.Cells[5].Value = "true";
                        MessageBox.Show("Commande validée.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la validation de la commande.");
                    }
                }
            }
        }

        public partial class FormClient : Form
            {
                public static List<List<string>> DataPlatsStatic;

                private List<string> Data;
                private List<List<string>> DataPlats;
                private Dictionary<int, int> PanierClient;
                private BDD BDD;
                int idClient;
            private Graphe graphe;

            private Connection connection;

            public FormClient(List<string> data, List<List<string>> dataPlats, Dictionary<int, int> panierClient, BDD bdd, int IdClient, Graphe graphe, Connection connection)
            {
                this.Data = data;
                this.DataPlats = dataPlats;
                this.PanierClient = panierClient;
                this.BDD = bdd;
                this.idClient = IdClient;
                this.graphe = graphe;
                this.connection = connection;
                DataPlatsStatic = dataPlats;
                InitUI();
            }



            private void InitUI()
            {
                this.Text = "Espace Client";
                this.Size = new System.Drawing.Size(400, 300);

                Label welcomeLabel = new Label
                {
                    Text = $"Bienvenue {Data[0]} {Data[1]} à Liv'In Paris",
                    AutoSize = true,
                    Location = new System.Drawing.Point(30, 20)
                };
                this.Controls.Add(welcomeLabel);

                Button btnPlats = new Button
                {
                    Text = "Plats Disponibles",
                    Location = new System.Drawing.Point(30, 60),
                    Size = new System.Drawing.Size(200, 30)
                };
                btnPlats.Click += BtnPlats_Click;
                this.Controls.Add(btnPlats);

                Button btnPanier = new Button
                {
                    Text = "Voir mon Panier",
                    Location = new System.Drawing.Point(30, 100),
                    Size = new System.Drawing.Size(200, 30)
                };
                btnPanier.Click += BtnPanier_Click;
                this.Controls.Add(btnPanier);

                Button btnCommandes = new Button
                {
                    Text = "Voir mes commandes",
                    Location = new System.Drawing.Point(30, 140),
                    Size = new System.Drawing.Size(200, 30)
                };
                btnCommandes.Click += BtnCommandes_Click;
                this.Controls.Add(btnCommandes);

                Button btnParametres = new Button
                {
                    Text = "Paramètres du compte",
                    Location = new System.Drawing.Point(30, 180),
                    Size = new System.Drawing.Size(200, 30)
                };
                btnParametres.Click += BtnParametres_Click;
                this.Controls.Add(btnParametres);

                Button btnDeconnexion = new Button
                {
                    Text = "Déconnexion",
                    Location = new System.Drawing.Point(30, 220),
                    Size = new System.Drawing.Size(200, 30)
                };
                btnDeconnexion.Click += (s, e) =>
                {
                    this.Close();
                    System.Threading.Thread t = new System.Threading.Thread(() =>
                    {
                        connection.OuvrirIU();
                    });
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                };
                this.Controls.Add(btnDeconnexion);
            }

            private void BtnPlats_Click(object sender, EventArgs e)
                {
                    this.DataPlats = BDD.GetDataPlats();
                    FormPlatsDisponibles form = new FormPlatsDisponibles(this.DataPlats, Data[0], Data[1], this.PanierClient, this.BDD);
                    form.ShowDialog();
                }

                private void BtnPanier_Click(object sender, EventArgs e)
                {
                    if (PanierClient.Count == 0)
                    {
                        MessageBox.Show("Panier vide.");
                    }
                    else
                    {

                        FormPanier panierForm = new FormPanier(idClient, DataPlats, PanierClient, BDD);


                    panierForm.ShowDialog();
                    }
                }

            private void BtnParametres_Click(object sender, EventArgs e)
            {
                Form form = new Form
                {
                    Text = "Paramètres du compte",
                    Size = new System.Drawing.Size(400, 500),
                    StartPosition = FormStartPosition.CenterScreen
                };

                // Champs
                Label[] labels = new Label[8];
                TextBox[] textboxes = new TextBox[8];
                string[] noms = { "Nom", "Prénom", "Adresse", "Code Postal", "Ville", "Téléphone", "Email", "Mot de passe" };

                for (int i = 0; i < noms.Length; i++)
                {
                    labels[i] = new Label { Text = noms[i], Left = 20, Top = 30 + i * 35, Width = 100 };
                    textboxes[i] = new TextBox { Left = 130, Top = 30 + i * 35, Width = 200, Text = Data[i] };
                    form.Controls.Add(labels[i]);
                    form.Controls.Add(textboxes[i]);
                }

                // Bouton changer station
                Button btnChangerStation = new Button { Text = "Changer station", Left = 20, Top = 330, Width = 310 };
                int selectedIdMetro = int.Parse(Data[9]);
                string selectedNomMetro = Data[10];
                btnChangerStation.Click += (s, ev) =>
                {
                    var result = graphe.SelectStation();
                    if (result != null && result.Count == 2)
                    {
                        selectedIdMetro = int.Parse(result[0]);
                        selectedNomMetro = result[1];
                        MessageBox.Show($"Nouvelle station : {selectedNomMetro} (ID {selectedIdMetro})");
                    }
                };
                form.Controls.Add(btnChangerStation);

                // Bouton Mettre à jour
                Button btnMaj = new Button { Text = "Mettre à jour", Left = 20, Top = 370, Width = 150 };
                btnMaj.Click += (s, ev) =>
                {
                    if (!int.TryParse(textboxes[3].Text.Trim(), out int cp))
                    {
                        MessageBox.Show("Code postal invalide");
                        return;
                    }

                    bool ok = BDD.MettreAJourClient(
                        idClient,
                        textboxes[0].Text, textboxes[1].Text, textboxes[2].Text,
                        cp, textboxes[4].Text, textboxes[5].Text,
                        textboxes[6].Text, textboxes[7].Text,
                        selectedIdMetro, selectedNomMetro
                    );

                    if (ok) MessageBox.Show("Mise à jour réussie");
                    else MessageBox.Show("Erreur lors de la mise à jour");
                };
                form.Controls.Add(btnMaj);

                // Bouton Supprimer compte
                Button btnSupprimer = new Button { Text = "Supprimer compte", Left = 180, Top = 370, Width = 150 };
                btnSupprimer.Click += (s, ev) =>
                {
                    var confirm = MessageBox.Show("Voulez-vous vraiment supprimer votre compte ?", "Confirmation", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        if (BDD.SupprimerClient(idClient))
                        {
                            MessageBox.Show("Compte supprimé.");
                            form.Close();
                            this.Close(); 

                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression.");
                        }
                    }
                };
                form.Controls.Add(btnSupprimer);

                form.ShowDialog();
            }



            private void BtnCommandes_Click(object sender, EventArgs e)
            {
                List<List<string>> commandes = BDD.GetDataCommandes(); // Rafraîchir les commandes
                List<List<string>> commandesClient = new List<List<string>>();

                foreach (var cmd in commandes)
                {
                    if (int.TryParse(cmd[1], out int clientId) && clientId == idClient)
                    {
                        commandesClient.Add(cmd);
                    }
                }

                if (commandesClient.Count == 0)
                {
                    MessageBox.Show("Vous n'avez encore passé aucune commande.");
                }
                else
                {
                    FormCommandes form = new FormCommandes(commandesClient, BDD, graphe);
                    form.ShowDialog();

                }
            }


        }

        private class FormPlatsDisponibles : Form
            {
                private List<List<string>> plats;
                private string clientNom;
                private string clientPrenom;
                private Dictionary<int, int> PanierClient;
                private DataGridView dataGrid;
                private BDD bdd;

                public FormPlatsDisponibles(List<List<string>> plats, string nom, string prenom, Dictionary<int, int> panierClient, BDD bdd)
                {
                    this.plats = plats;
                    this.clientNom = nom;
                    this.clientPrenom = prenom;
                    this.PanierClient = panierClient;
                    this.bdd = bdd;
                    InitUI();
                }

                private void InitUI()
                {
                    this.Text = "Plats Disponibles";
                    this.Size = new System.Drawing.Size(800, 400);

                    Label welcome = new Label
                    {
                        Text = $"Bienvenue {clientPrenom} {clientNom}, voici les plats disponibles :",
                        AutoSize = true,
                        Location = new System.Drawing.Point(20, 20)
                    };
                    this.Controls.Add(welcome);

                    dataGrid = new DataGridView
                    {
                        Location = new System.Drawing.Point(20, 60),
                        Size = new System.Drawing.Size(740, 250),
                        AllowUserToAddRows = false,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

                    dataGrid.Columns.Add("Nom", "Nom");
                    dataGrid.Columns.Add("Description", "Description");
                    dataGrid.Columns.Add("QuantiteDispo", "Quantité Disponible");
                    dataGrid.Columns.Add("Quantite", "Quantité à commander");
                    dataGrid.Columns.Add("Prix", "Prix (€)");

                    DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn
                    {
                        HeaderText = "Action",
                        Text = "Ajouter au panier",
                        UseColumnTextForButtonValue = true
                    };
                    dataGrid.Columns.Add(btnCol);

                    foreach (var plat in plats)
                    {
                        if (int.TryParse(plat[3], out int nb) && nb > 0)
                        {
                            dataGrid.Rows.Add(plat[1], plat[4], plat[3], "1", plat[5]);

                        }
                    }

                    dataGrid.CellContentClick += DataGrid_CellContentClick;
                    this.Controls.Add(dataGrid);
                }

                private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
                {
                    if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                    {
                        string nomPlat = dataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                        string quantiteStr = dataGrid.Rows[e.RowIndex].Cells[3].Value.ToString();
                        string disponibleStr = dataGrid.Rows[e.RowIndex].Cells[2].Value.ToString();

                        if (int.TryParse(quantiteStr, out int quantite) && quantite > 0 &&
                            int.TryParse(disponibleStr, out int quantiteDisponible))
                        {
                            if (quantite > quantiteDisponible)
                            {
                                MessageBox.Show("Quantité demandée supérieure à la quantité disponible !");
                                return;
                            }

                            int idPlat = -1;
                            foreach (var plat in plats)
                            {
                                if (plat[1] == nomPlat)
                                {
                                    idPlat = int.Parse(plat[0]);
                                    break;
                                }
                            }

                            if (idPlat != -1)
                            {
                                if (PanierClient.ContainsKey(idPlat))
                                    PanierClient[idPlat] += quantite;
                                else
                                    PanierClient[idPlat] = quantite;

                                int nouvelleQuantite = quantiteDisponible - quantite;
                                dataGrid.Rows[e.RowIndex].Cells[2].Value = nouvelleQuantite.ToString();

                                bdd.ModifierQuantitePlat(idPlat, nouvelleQuantite);
                                MessageBox.Show($"{quantite} x {nomPlat} ajouté(s) au panier !");
                            }
                            else
                            {
                                MessageBox.Show("Erreur : plat introuvable dans la base de données.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Quantité invalide !");
                        }
                    }
                }
            }

        private class FormPanier : Form
        {
            private int idClient;
            private List<List<string>> dataPlats;
            private Dictionary<int, int> panierClient;
            private BDD bdd;

            public FormPanier(int idClient, List<List<string>> dataPlats, Dictionary<int, int> panierClient, BDD bdd)
            {
                this.idClient = idClient;
                this.dataPlats = dataPlats;
                this.panierClient = panierClient;
                this.bdd = bdd;

                this.Text = "Mon Panier";
                this.Size = new System.Drawing.Size(500, 400);

                Label title = new Label
                {
                    Text = "Contenu de votre panier :",
                    AutoSize = true,
                    Location = new System.Drawing.Point(20, 20)
                };
                this.Controls.Add(title);

                DataGridView dgv = new DataGridView
                {
                    Location = new System.Drawing.Point(20, 50),
                    Size = new System.Drawing.Size(440, 200),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                dgv.Columns.Add("Nom", "Nom");
                dgv.Columns.Add("Quantite", "Quantité");
                dgv.Columns.Add("PrixUnitaire", "Prix Unitaire (€)");
                dgv.Columns.Add("Total", "Total (€)");

                double totalPrix = 0;

                foreach (var item in panierClient)
                {
                    string nomPlat = "Inconnu";
                    double prixUnitaire = 0;

                    foreach (var plat in dataPlats)
                    {
                        if (int.Parse(plat[0]) == item.Key)
                        {
                            nomPlat = plat[1];
                            string prixStr = plat[5].Replace(',', '.').Trim();
                            double.TryParse(prixStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out prixUnitaire);
                            break;
                        }
                    }

                    int quantite = item.Value;
                    double total = prixUnitaire * quantite;
                    totalPrix += total;

                    dgv.Rows.Add(nomPlat, quantite, prixUnitaire.ToString("0.00"), total.ToString("0.00"));
                }

                this.Controls.Add(dgv);

                Label lblTotal = new Label
                {
                    Text = $"Total à payer : {totalPrix.ToString("0.00")} €",
                    AutoSize = true,
                    Location = new System.Drawing.Point(20, 270),
                    Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
                };
                this.Controls.Add(lblTotal);

                Button btnPayer = new Button
                {
                    Text = "Payer",
                    Location = new System.Drawing.Point(370, 310),
                    Size = new System.Drawing.Size(90, 30)
                };

                btnPayer.Click += (sender, e) =>
                {
                    bool success = true;
                    string dateCommande = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    foreach (var item in panierClient)
                    {
                        int idPlat = item.Key;
                        int quantite = item.Value;
                        double prixUnitaire = 0;
                        int idCuisinier = 0;

                        foreach (var plat in dataPlats)
                        {
                            if (int.Parse(plat[0]) == idPlat)
                            {
                                string prixStr = plat[5].Replace(',', '.').Trim();
                                if (!double.TryParse(prixStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out prixUnitaire))
                                {
                                    MessageBox.Show($"Erreur : prix invalide pour le plat '{plat[1]}' (valeur : '{plat[5]}')");
                                    success = false;
                                    break;
                                }

                                if (!int.TryParse(plat[2], out idCuisinier))
                                {
                                    MessageBox.Show($"Erreur : id cuisinier invalide pour le plat '{plat[1]}'");
                                    success = false;
                                    break;
                                }
                                break;
                            }
                        }

                        if (!success) break;

                        for (int i = 0; i < quantite; i++)
                        {
                            if (!bdd.AjouterCommande(idClient, idPlat, idCuisinier, prixUnitaire, dateCommande))
                            {
                                MessageBox.Show($"Erreur lors de l'ajout de la commande pour le plat ID {idPlat}");
                                success = false;
                                break;
                            }
                        }

                        if (!success) break;
                    }

                    if (success)
                    {
                        double montantTotal = 0;
                        foreach (var item in panierClient)
                        {
                            foreach (var plat in dataPlats)
                            {
                                if (int.Parse(plat[0]) == item.Key)
                                {
                                    string prixStr = plat[5].Replace(',', '.').Trim();
                                    if (double.TryParse(prixStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double prix))
                                    {
                                        montantTotal += prix * item.Value;
                                    }
                                    break;
                                }
                            }
                        }

                        panierClient.Clear();
                        MessageBox.Show($"Vous avez payé {montantTotal:0.00} €, commande passée avec succès !");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Une erreur est survenue lors du paiement.");
                    }
                };

                this.Controls.Add(btnPayer);
            }
        }
        private class FormCommandes : Form
        {
            private BDD bdd;
            private Graphe graphe;
            private List<List<string>> commandes;
            private DataGridView dgv;

            public FormCommandes(List<List<string>> commandes, BDD bdd, Graphe graphe)
            {
                this.commandes = commandes;
                this.bdd = bdd;
                this.graphe = graphe;

                this.Text = "Mes Commandes";
                this.Size = new System.Drawing.Size(1000, 500);
                InitUI();
            }

            private void InitUI()
            {
                dgv = new DataGridView
                {
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(1000, 400),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                dgv.Columns.Add("NomPlat", "Nom du plat");
                dgv.Columns.Add("NomCuisinier", "Cuisinier");
                dgv.Columns.Add("AdresseCuisinier", "Adresse");
                dgv.Columns.Add("DateCommande", "Date de commande");
                dgv.Columns.Add("Prix", "Prix (€)");
                dgv.Columns.Add("Livre", "Livré ?");

                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn
                {
                    HeaderText = "Trajet",
                    Text = "Afficher Trajet",
                    UseColumnTextForButtonValue = true
                };
                dgv.Columns.Add(btnCol);

                foreach (var cmd in commandes)
                {
                    string nomPlat = "Inconnu";
                    string nomCuisinier = "Inconnu";
                    string adresseCuisinier = "Inconnue";
                    string prix = cmd[4];
                    string date = cmd[5];
                    string livre = cmd.Count > 6 && cmd[6].ToLower() == "true" ? "Oui" : "Non";

                    int idPlat = int.Parse(cmd[2]);
                    int idCuisinier = int.Parse(cmd[3]);

                    // Recherche du nom du plat
                    foreach (var plat in Session.FormClient.DataPlatsStatic)
                    {
                        if (int.Parse(plat[0]) == idPlat)
                        {
                            nomPlat = plat[1];
                            break;
                        }
                    }

                    // Recherche des infos du cuisinier
                    var infoCuisinier = bdd.GetCuisinierInfo(idCuisinier);
                    if (infoCuisinier != null)
                    {
                        nomCuisinier = $"{infoCuisinier[0]} {infoCuisinier[1]}";
                        adresseCuisinier = infoCuisinier[2];
                    }

                    dgv.Rows.Add(nomPlat, nomCuisinier, adresseCuisinier, date, prix, livre);
                }

                dgv.CellContentClick += Dgv_CellContentClick;
                this.Controls.Add(dgv);
            }


            private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                {
                    try
                    {
                        var cmd = commandes[e.RowIndex];
                        int idClient = int.Parse(cmd[1]);
                        int idCuisinier = int.Parse(cmd[3]);

                        int idMetroClient = bdd.GetMetroId(idClient, 0);
                        int idMetroCuisinier = bdd.GetMetroId(0, idCuisinier);

                        if (idMetroClient == -1 || idMetroCuisinier == -1)
                        {
                            MessageBox.Show("Station métro non trouvée pour client ou cuisinier.");
                            return;
                        }

                        var trajet = graphe.TrouverTrajet(idMetroClient, idMetroCuisinier);
                        string message = string.Join(" ➜ ", trajet);
                        MessageBox.Show(message, "Trajet entre Client et Cuisinier");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du calcul du trajet : " + ex.Message);
                    }
                }
            }
        }




        private class ChoixR : Form
            {
                public string ChosenRole { get; private set; }

                public ChoixR(string nom, string prenom)
                {
                    Text = "Choix du rôle";
                    Width = 350;
                    Height = 150;
                    FormBorderStyle = FormBorderStyle.FixedDialog;
                    StartPosition = FormStartPosition.CenterScreen;
                    MaximizeBox = false;
                    MinimizeBox = false;

                    Label label = new Label
                    {
                        Text = $"Bonjour {prenom} {nom},\n\nSouhaitez-vous accéder à :",
                        Dock = DockStyle.Top,
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                        Height = 50
                    };

                    Button btnClient = new Button
                    {
                        Text = "Espace Client",
                        Left = 40,
                        Width = 120,
                        Top = 70
                    };
                    btnClient.Click += (sender, e) => { ChosenRole = "Client"; Close(); };

                    Button btnCuisinier = new Button
                    {
                        Text = "Espace Cuisinier",
                        Left = 180,
                        Width = 120,
                        Top = 70
                    };
                    btnCuisinier.Click += (sender, e) => { ChosenRole = "Cuisinier"; Close(); };

                    Controls.Add(label);
                    Controls.Add(btnClient);
                    Controls.Add(btnCuisinier);
                }
            }
        }
    }
