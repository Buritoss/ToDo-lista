using System;
using System.Collections.Generic;
using System.IO;
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
using System.Data.SQLite;
using Dapper;
using System.CodeDom;

namespace ToDoBD
{
    /// <summary>
    /// Interaction logic for ToDoList.xaml
    /// </summary>
    public partial class ToDoList : Page
    {
        List<DockPanel> listOfDocks= new List<DockPanel>();
        List<TextBox> listOfTexts= new List<TextBox>();
        List<DatePicker> listOfDates= new List<DatePicker>();
        MainWindow owner;


        public ToDoList(MainWindow owner)
        {
            InitializeComponent();
            this.owner = owner;
        }

        private void AddThingToDo(object sender, RoutedEventArgs e)
        {
            DockPanel dp = new DockPanel();
            Button removeButton = new Button();
            TextBox toDoText = new TextBox();
            DatePicker datep = new DatePicker();


            removeButton.Padding = new Thickness(10,10,10,10);
            removeButton.Content = "X";
            removeButton.Background = Brushes.Red;
            removeButton.BorderThickness = new Thickness(3);
            removeButton.BorderBrush = Brushes.Black;
            removeButton.Click += DeleteYourOwnDock;


            dp.Children.Add(removeButton);
            dp.Children.Add(datep);
            dp.Children.Add(toDoText);


            listOfDates.Add(datep);
            listOfTexts.Add(toDoText);
            listOfDocks.Add(dp);

            listOfThings.Children.Add(dp);
        }

        private void DeleteYourOwnDock(object sender, RoutedEventArgs e)
        {
            listOfThings.Children.Remove((DockPanel)((sender as Button).Parent));

            for(int i =0;i< listOfDocks.Count ;i++)
            {
                if (listOfDocks[i] == (DockPanel)((sender as Button).Parent))
                {
                    listOfDocks.RemoveAt(i);
                    listOfDates.RemoveAt(i);
                    listOfTexts.RemoveAt(i);
                }
            }
               
        }
           
        

        private void AddTableToDataBase(object sender, RoutedEventArgs e)
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                string sqlCommand = String.Format("Create table '{0}' (id INTEGER PRIMARY KEY AUTOINCREMENT, date varchar(255), task varchar(255));",
                    titleOfToDoList.Text);

                conn.Query<string>(sqlCommand);


                
                for (int i=0;i<listOfDocks.Count ;i++)
                {
                    conn.Query<string>(String.Format("INSERT INTO {0} VALUES(NULL,'{1}','{2}');",
                        titleOfToDoList.Text,
                        listOfDates[i].Text,
                        listOfTexts[i].Text));
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoList.cs");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }

            owner.PrintListOfToDoLists(new object(), new RoutedEventArgs());
        }


        private void ClearWholeDataBase()
        {
            string path = "./ToDoList.db";
            var conn = new SQLiteConnection(string.Format("Data Source={0}", path));

            try
            {
                conn.Open();

                var q = conn.Query<string>("SELECT name FROM sqlite_master WHERE type = 'table';");
                MessageBox.Show(q.Count().ToString());
                foreach (var item in q)
                    if( item.ToString() != "sqlite_sequence")
                        conn.Query<string>("drop table'" + item.ToString()+"';") ;
                
                MessageBox.Show(q.Count().ToString());
            }
            catch (Exception ex)
            {

                Console.WriteLine("--------------------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------");
            }
            finally
            { conn.Close(); }
        }

    }
}
