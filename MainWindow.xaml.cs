using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using System.Data.SQLite;
using System.Net.Mail;

namespace ToDoBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Button> listOfButtons = new List<Button>();
        public MainWindow()
        {
            InitializeComponent();
            PrintListOfToDoLists( new object(), new RoutedEventArgs());
            AddToDoList(new object(),new RoutedEventArgs());

            if(Properties.Settings.Default.email != null)
                emailBox.Text = Properties.Settings.Default.email;

            //CheckDates();
        }

        public void AddToDoList(object sender, RoutedEventArgs e)
            =>mainView.Content = new ToDoList(this);
        

        public void PrintListOfToDoLists(object sender, RoutedEventArgs e)
        {
            RemoveToDoListsFromList();
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = "SELECT name FROM sqlite_master WHERE type = 'table' AND name != 'sqlite_sequence';";

                var names = conn.Query<string>(sqlCommand);

                foreach(var name in names)
                {
                    Button linkButton = new Button();

                    linkButton.Content = name;
                    linkButton.Click += CreateToDoPage;
                    //linkButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#bababa");
                    linkButton.Background = Brushes.White;
                    linkButton.BorderBrush = Brushes.Black;
                    linkButton.BorderThickness = new Thickness(3);
                    listOfButtons.Add(linkButton);
                    listOfToDoLists.Children.Add(linkButton);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainWindow.cs");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
        }

        private void RemoveToDoListsFromList()
        {
            foreach(Button b in listOfButtons)
            {
                listOfToDoLists.Children.Remove(b);
            }
        }

        private void CreateToDoPage(object sender, RoutedEventArgs e)
            =>mainView.Content = new OneOfLists((sender as Button).Content.ToString(), this);

        private void UpdateEmail(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.email = (sender as TextBox).Text;
            Properties.Settings.Default.Save();
        }

        private void CheckDates()
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = "SELECT name FROM sqlite_master WHERE type = 'table' AND name != 'sqlite_sequence';";

                var names = conn.Query<string>(sqlCommand);

                foreach (var name in names)
                {
                    sqlCommand = String.Format("SELECT id, task, date from '{0}'",
                        name);

                    var rows = conn.Query<dataFromToDoList>(sqlCommand);

                    foreach(var row in rows)
                    {
                        if (row.date == DateTime.Now.ToString("dd.mm.yyyy"))
                            SendEmail(row.task, row.date);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainWindow.cs/chackDates");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
        }

        private void SendEmail(string task, string date)
        {
            /*
            string to = emailBox.Text;
            string from = "noreply@contoso.com";

            MailMessage message = new MailMessage(from, to);

            message.Subject = "test";
            message.Body = "test2";
            SmtpClient client = new SmtpClient(server);
            client.UseDefaultCredentials = true;
            */
        }
    }
}
