using System;
using System.Windows.Forms;
using System.Drawing;

namespace Projet_PSI
{
    internal class Connection
    {
        string BDD;
        Graphe graphe;
        BDD BDD1; // À adapter si tu as une classe spécifique

        public Connection(string BDD,Graphe graphe)
        {
            this.BDD = BDD;
            this.graphe = graphe;
            this.BDD1 = new BDD(BDD); // Remplace par ta classe réelle de gestion BDD
        }

        public void OuvrirIU()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm(this));
        }

        private class LoginForm : Form
        {
            private Label welcomeLabel;
            private TextBox emailTextBox;
            private TextBox passwordTextBox;
            private Button loginButton;
            private Label emailLabel;
            private Label passwordLabel;
            private LinkLabel registerLinkLabel;
            private Connection connection;

            public LoginForm(Connection connection)
            {
                this.connection = connection;

                this.Text = "Connexion";
                this.Width = 320;
                this.Height = 250;

                welcomeLabel = new Label()
                {
                    Left = 50,
                    Top = 10,
                    Width = 200,
                    Text = "Bienvenue à Liv'In Paris",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                welcomeLabel.Left = (this.ClientSize.Width - welcomeLabel.Width) / 2;

                emailLabel = new Label() { Left = 10, Top = 50, Text = "Email" };
                emailTextBox = new TextBox() { Left = 110, Top = 50, Width = 180 };

                passwordLabel = new Label() { Left = 10, Top = 90, Text = "Mot de passe" };
                passwordTextBox = new TextBox() { Left = 110, Top = 90, Width = 180, PasswordChar = '*' };

                loginButton = new Button() { Text = "Se connecter", Left = 110, Top = 130, Width = 180 };
                loginButton.Click += LoginButton_Click;

                registerLinkLabel = new LinkLabel()
                {
                    Text = "Créer un compte",
                    Left = 100,
                    Top = 170,
                    Width = 180,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                registerLinkLabel.Click += RegisterLinkLabel_Click;

                this.Controls.Add(welcomeLabel);
                this.Controls.Add(emailLabel);
                this.Controls.Add(emailTextBox);
                this.Controls.Add(passwordLabel);
                this.Controls.Add(passwordTextBox);
                this.Controls.Add(loginButton);
                this.Controls.Add(registerLinkLabel);
            }

            private void LoginButton_Click(object sender, EventArgs e)
            {
                string email = emailTextBox.Text.Trim();
                string password = passwordTextBox.Text.Trim();

                int idClient = connection.BDD1.CheckPassClient(email, password);
                int idCuisinier = connection.BDD1.CheckPassCuisinier(email, password);

                if (idClient != 0 || idCuisinier != 0)
                {
                    this.Hide();
                    Session session = new Session(idClient, idCuisinier, connection.BDD, connection.graphe, connection);


                    session.OpenSession();
                }
                else
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void RegisterLinkLabel_Click(object sender, EventArgs e)
            {
                this.Hide();
                RegisterForm registerForm = new RegisterForm(connection);
                registerForm.FormClosed += (s, args) => this.Show();
                registerForm.Show();
            }
        }

        private class RegisterForm : Form
        {
            private TextBox nomTextBox, prenomTextBox, adresseTextBox, codePostalTextBox, villeTextBox, telephoneTextBox, emailTextBox, motDePasseTextBox;
            private CheckBox clientCheckBox, cuisinierCheckBox;
            private Button registerButton, selectMetroButton;
            private Connection connection;
            private int idMetro = -1;
            private string nomMetro = "";

            public RegisterForm(Connection connection)
            {
                this.connection = connection;

                this.Text = "Créer un compte";
                this.Width = 450;
                this.Height = 600;

                int labelLeft = 20;
                int inputLeft = 150;
                int top = 20;
                int spacing = 30;

                void AddLabel(string text, int y) => this.Controls.Add(new Label { Text = text, Left = labelLeft, Top = y, Width = 120 });
                TextBox AddTextBox(int y)
                {
                    var tb = new TextBox { Left = inputLeft, Top = y, Width = 200 };
                    this.Controls.Add(tb);
                    return tb;
                }

                AddLabel("Nom", top); nomTextBox = AddTextBox(top);
                top += spacing; AddLabel("Prénom", top); prenomTextBox = AddTextBox(top);
                top += spacing; AddLabel("Adresse", top); adresseTextBox = AddTextBox(top);
                top += spacing; AddLabel("Code Postal", top); codePostalTextBox = AddTextBox(top);
                top += spacing; AddLabel("Ville", top); villeTextBox = AddTextBox(top);
                top += spacing; AddLabel("Téléphone", top); telephoneTextBox = AddTextBox(top);
                top += spacing; AddLabel("Email", top); emailTextBox = AddTextBox(top);
                top += spacing; AddLabel("Mot de passe", top); motDePasseTextBox = AddTextBox(top);
                motDePasseTextBox.PasswordChar = '*';

                // Bouton sélection métro
                top += spacing + 10;
                selectMetroButton = new Button
                {
                    Text = "Sélectionner station de métro",
                    Left = labelLeft,
                    Top = top,
                    Width = 330
                };
                selectMetroButton.Click += SelectMetroButton_Click;
                this.Controls.Add(selectMetroButton);

                // Checkboxes
                top += spacing + 20;
                clientCheckBox = new CheckBox { Text = "S'inscrire en tant que client", Left = labelLeft, Top = top, Width = 250 };
                cuisinierCheckBox = new CheckBox { Text = "S'inscrire en tant que cuisinier", Left = labelLeft, Top = top + 30, Width = 250 };
                this.Controls.Add(clientCheckBox);
                this.Controls.Add(cuisinierCheckBox);

                // Bouton d'inscription
                top += 80;
                registerButton = new Button { Text = "S'inscrire", Left = 120, Top = top, Width = 150 };
                registerButton.Click += RegisterButton_Click;
                this.Controls.Add(registerButton);
            }

            private void SelectMetroButton_Click(object sender, EventArgs e)
            {
                var result = connection.graphe.SelectStation();
                if (result != null && result.Count == 2)
                {
                    idMetro = int.Parse(result[0]);
                    nomMetro = result[1];
                    MessageBox.Show($"Station sélectionnée : {nomMetro} (ID {idMetro})", "Station sélectionnée");
                }
            }

            private void RegisterButton_Click(object sender, EventArgs e)
            {
                string nom = nomTextBox.Text;
                string prenom = prenomTextBox.Text;
                string adresse = adresseTextBox.Text;
                string codePostalStr = codePostalTextBox.Text;
                string ville = villeTextBox.Text;
                string telephone = telephoneTextBox.Text;
                string email = emailTextBox.Text;
                string motDePasse = motDePasseTextBox.Text;

                if (!int.TryParse(codePostalStr, out int codePostal))
                {
                    MessageBox.Show("Code postal invalide.");
                    return;
                }

                if (idMetro == -1 || string.IsNullOrEmpty(nomMetro))
                {
                    MessageBox.Show("Veuillez sélectionner une station de métro.");
                    return;
                }

                if (!(clientCheckBox.Checked || cuisinierCheckBox.Checked))
                {
                    MessageBox.Show("Veuillez cocher au moins un type de compte.");
                    return;
                }

                if (connection.BDD1.CheckMail(email))
                {
                    MessageBox.Show("Cet email est déjà utilisé.");
                    return;
                }

                bool ajoutOk = false;

                if (clientCheckBox.Checked)
                    ajoutOk = connection.BDD1.AjouterClient(nom, prenom, adresse, codePostal, ville, telephone, email, motDePasse, false, idMetro, nomMetro);

                if (cuisinierCheckBox.Checked)
                    ajoutOk = connection.BDD1.AjouterCuisinier(nom, prenom, adresse, codePostal, ville, telephone, email, motDePasse, false, idMetro, nomMetro);

                if (ajoutOk)
                {
                    MessageBox.Show("Inscription réussie !");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'inscription.");
                }
            }
        }

    }
}
