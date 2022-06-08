using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        ICommonAccess module;
        public TypeOfUser userType { get; set; }
        public Registration(ICommonAccess module)
        {
            userType = TypeOfUser.Undefined;
            this.module = module;
            InitializeComponent();
        }

        private void BtnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if(!EditPassword.Password.Equals(EditPasswordRepeat.Password))
            {
                MessageBox.Show("Пароли должны совпадать");
            }

            if (module.Registration<Client>(TypeOfUser.Client, new Client(EditLogin.Text, EditPassword.Password)) == AnswerFromServer.Successfully) { userType = TypeOfUser.Client; MessageBox.Show("Успешно"); Close(); }
            else MessageBox.Show("Аккаунт с тким логином уже существует");
        }
    }
}
