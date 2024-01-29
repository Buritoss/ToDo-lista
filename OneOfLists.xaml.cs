using Dapper;
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

namespace ToDoBD
{
    /// <summary>
    /// Interaction logic for OneOfLists.xaml
    /// </summary>
    public partial class OneOfLists : Page
    {
        string tableName= string.Empty;
        MainWindow owner;
        public OneOfLists(string title, MainWindow owner)
        {
            InitializeComponent();
            tableName= title;
            this.owner = owner;

            tableNameXAML.Content= tableName;
            ConnectToToDoList();
        }

        private void ConnectToToDoList()
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = "SELECT name FROM sqlite_master WHERE type = 'table';";

                var names = conn.Query<string>(sqlCommand);

                
                var tableData = conn.Query<dataFromToDoList>(String.Format("select id,date,task from '{0}';",
                    tableName)).ToList();


                foreach(var data in tableData)
                    PrintToDoList(data.id,data.task, data.date);
            }
            catch (Exception ex)
            {
                Console.WriteLine("OneOfLists.cs/ConnectToToDoList");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
        }

        private void PrintToDoList(int id,string task, string date)
        {
            DockPanel dockPanel = new DockPanel();
            TextBlock taskTextBlock = new TextBlock();
            TextBlock dateTextBlock = new TextBlock();
            Button removeButton = new Button();

            removeButton.Content = id.ToString();
            removeButton.Margin = new Thickness(15);
            removeButton.Padding = new Thickness(15);
            removeButton.Click += RemoveRowFromTable;
            removeButton.Background = Brushes.Red;
            removeButton.BorderThickness = new Thickness(3);
            removeButton.BorderBrush = Brushes.Black;

            taskTextBlock.Text = task;
            taskTextBlock.Margin = new Thickness(15);
            taskTextBlock.Background = Brushes.White;
            taskTextBlock.VerticalAlignment = VerticalAlignment.Center;
            taskTextBlock.Padding = new Thickness(10);
           

            dateTextBlock.Text = date;
            dateTextBlock.Margin = new Thickness(15);
            dateTextBlock.Background = Brushes.White;
            dateTextBlock.VerticalAlignment = VerticalAlignment.Center;
            dateTextBlock.Padding = new Thickness(10);


            dockPanel.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#000080");
           


            dockPanel.Children.Add(removeButton);
            dockPanel.Children.Add(taskTextBlock);
            dockPanel.Children.Add(dateTextBlock);

            listOfThings.Children.Add(dockPanel);
        }

        private void RemoveRowFromTable(object sender, RoutedEventArgs e)
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = String.Format("Delete from {0} where id = {1}",
                    tableName,
                    (sender as Button).Content);

                conn.Query<string>(sqlCommand);

                listOfThings.Children.Remove((DockPanel)(sender as Button).Parent);


            }
            catch (Exception ex)
            {
                Console.WriteLine("OneOfLists.cs/RemoveRowFromTable");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
        }

        private void DropWholeTable(object sender, RoutedEventArgs e)
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = String.Format("DROP TABLE '{0}'",
                    tableName);

                conn.Query<string>(sqlCommand);
                this.Content = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("OneOfLists.cs/DropWholeTable");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
            owner.PrintListOfToDoLists(new Object(),new RoutedEventArgs());
            owner.AddToDoList(new object(), new RoutedEventArgs());
        }

        private void AddElementToList(object sender, RoutedEventArgs e)
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string qD = quickAddDate.Text;
                string qT = quickAddTask.Text;

                string sqlCommand = String.Format("INSERT INTO '{0}' VALUES(null,'{1}','{2}')",
                    tableName,
                    qD,
                    qT);

                conn.Query<string>(sqlCommand);

                


            }
            catch (Exception ex)
            {
                Console.WriteLine("OneOfLists.cs/AddElementToList");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
            listOfThings.Children.Clear();
            ConnectToToDoList();
        }

    }

    
}
