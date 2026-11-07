using System;
using System.Collections.Generic;
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
using Library_CP.database;
using Library_CP.viewmodels;

namespace Library_CP.views.pages
{
    /// <summary>
    /// Логика взаимодействия для MyBooksPage.xaml
    /// </summary>
    public partial class MyBooksPage : Page
    {
        public MyBooksPage()
        {
            InitializeComponent();
            DataContext = new BiblioSysDBEntities();
        }
    }
}
