using ClassLibraryForTCPConnectionAPI_C_sharp_;
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
using System.Windows.Shapes;
using TCPConnectionAPIClientModule_C_sharp_;

namespace Pattern2
{
    /// <summary>
    /// Interaction logic for Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        ICommonAccess module;
        public TypeOfUser typeOfUser { get; private set; }
        public Authorization(ICommonAccess module)
        {
            typeOfUser = TypeOfUser.Undefined;
            this.module = module;
            InitializeComponent();
        }

        private void BtnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            switch (module.Authorization(EditLogin.Text, EditPassword.Password))
            {
                case TypeOfUser.Admin:
                    {
                        MessageBox.Show("Вы авторизовались как администратор");
                        typeOfUser = TypeOfUser.Admin;
                        this.Close();
                        break;
                    }
                case TypeOfUser.Client:
                    {
                        MessageBox.Show("Вы авторизовались как пользователь");
                        typeOfUser = TypeOfUser.Client;
                        this.Close();
                        break;
                    }
                case TypeOfUser.Expert:
                    {
                        MessageBox.Show("Вы авторизовались как эксперт");
                        typeOfUser = TypeOfUser.Expert;
                        this.Close();
                        break;
                    }
                case TypeOfUser.Undefined:
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        typeOfUser = TypeOfUser.Undefined;
                        break;
                    }
                default:
                    break;
            }
        }

        private void BtnRegistration_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration(module);
            registration.ShowDialog();
            if (registration.userType == TypeOfUser.Client)
            {
                typeOfUser = TypeOfUser.Client;
                Close();
            }
        }
    }
}
