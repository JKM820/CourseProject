using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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
using TCPConnectionAPIClientModule_C_sharp_;

namespace Pattern2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public class UserItem
        {
            public UserItem(Admin a)
            {
                Login = a.Login;
                Status = "Админ";
                IsBanned = a.UserStatus == DatabaseEntities.Status.Banned;
            }

            public UserItem(Expert a)
            {
                Login = a.Login;
                Status = "Эксперт";
                IsBanned = a.UserStatus == DatabaseEntities.Status.Banned;
            }

            public UserItem(Client a)
            {
                Login = a.Login;
                Status = "Клиент";
                IsBanned = a.UserStatus == DatabaseEntities.Status.Banned;
            }
            public UserItem(String l, String S, bool isBanned)
            {
                Login = l;
                Status = S;
                IsBanned = isBanned;
            }
            public String Login { get; set; }
            public String Status { get; set; }
            public bool IsBanned { get; set; }
        }

        TypeOfUser typeOfUser;
        String currentLogin;

        ClientConnectionModule module;

        bool bEditedProduct;
        bool bEditedEmployee;
        bool bEditedUser;

        String strSearchProductName;
        int nMinProductCost;
        int nMaxProductCost;

        String strSearchEmployeeName;
        int nMinEmployeeExperience;
        int nMaxEmployeeExperience;

        String strSearchUserLogin;

        int selEmployeeIndex;

        // Флаги видимости окон в приложении
        bool bShowProductControl;
        bool bShowEmployeeControl;
        bool bShowUserControl;
        bool bShowExpertMethod;

        // Флаги видимости субэлементов в управлении продуктами
        bool bShowSearchProduct;
        bool bShowFilterProduct;

        bool bShowSearchEmployee;
        bool bShowFilterEmployee;

        bool bShowSearchUser;
        bool bShowAddUser;

        List<Detail> details;
        List<Detail> subdetails;

        List<Creditor> creditors;
        List<Creditor> subcreditors;

        List<Admin> admins;
        List<Client> clients;
        List<Expert> experts;
        List<UserItem> userItems;

        public MainWindow()
        {
            module = new ClientConnectionModule();

            InitializeComponent();

            OnStartSession();
        }

        private void OnStartSession()
        {
            typeOfUser = TypeOfUser.Undefined;
            Authorization authorization = new Authorization(module);
            authorization.ShowDialog();
            typeOfUser = authorization.typeOfUser;
            switch (typeOfUser)
            {
                case ClassLibraryForTCPConnectionAPI_C_sharp_.TypeOfUser.Admin:
                    BtnUser.IsEnabled = true;
                    BtnExpert.IsEnabled = false;
                    break;
                case ClassLibraryForTCPConnectionAPI_C_sharp_.TypeOfUser.Expert:
                    BtnUser.IsEnabled = false;
                    BtnExpert.IsEnabled = true;
                    break;
                case ClassLibraryForTCPConnectionAPI_C_sharp_.TypeOfUser.Client:
                    BtnUser.IsEnabled = false;
                    BtnExpert.IsEnabled = false;
                    break;
                case ClassLibraryForTCPConnectionAPI_C_sharp_.TypeOfUser.Undefined:
                    Close();
                    break;
                default:
                    break;
            }

            subdetails = new List<Detail>();
            details = new List<Detail>();

            creditors = new List<Creditor>();
            subcreditors = new List<Creditor>();

            admins = new List<Admin>();
            clients = new List<Client>();
            experts = new List<Expert>();
            userItems = new List<UserItem>();

            bEditedProduct = false;
            bEditedEmployee = false;
            bEditedUser = false;

            bShowProductControl = false;
            bShowEmployeeControl = false;
            bShowUserControl = false;
            bShowExpertMethod = false;

            bShowSearchProduct = false;
            bShowFilterProduct = false;

            bShowSearchEmployee = false;
            bShowFilterEmployee = false;

            bShowSearchUser = false;
            bShowAddUser = false;

            DGPUsers.ItemsSource = userItems;
            DGEmployee.ItemsSource = subcreditors;

            selEmployeeIndex = -1;

            strSearchProductName = "";
            nMinProductCost = -1;
            nMaxProductCost = -1;

            strSearchEmployeeName = "";
            nMinEmployeeExperience = -1;
            nMaxEmployeeExperience = -1;

            strSearchUserLogin = "";

            UpdateVisibility();
        }

        // Метод для обновления видимости 
        private void UpdateVisibility()
        {
            ProductControl.Visibility = Visibility.Collapsed;
            EmployeeControl.Visibility = Visibility.Collapsed;
            UserControl.Visibility = Visibility.Collapsed;
            ExpertMethod.Visibility = Visibility.Collapsed;

            if(bShowProductControl)
            {
                ProductControl.Visibility = Visibility.Visible;

                if(typeOfUser == TypeOfUser.Admin)
                {
                    DGProduct.CanUserAddRows = (!bShowSearchProduct && !bShowFilterProduct);
                    DGProduct.CanUserDeleteRows = !bShowSearchProduct && !bShowFilterProduct;
                    DGProduct.IsReadOnly = bShowSearchProduct || bShowFilterProduct;
                }
                else
                {
                    DGProduct.CanUserAddRows = false;
                    DGProduct.CanUserDeleteRows = false;
                    DGProduct.IsReadOnly = true;
                }

                SearchProduct.Visibility = (bShowSearchProduct ? Visibility.Visible : Visibility.Collapsed);
                if (!bShowSearchProduct)
                {
                    SearchName.Text = "Название";
                    strSearchProductName = "";
                }

                FilterProduct.Visibility = (bShowFilterProduct ? Visibility.Visible : Visibility.Collapsed);
                if (!bShowFilterProduct)
                {
                    FilterFirstFrom.Text = "От";
                    FilterFirstTo.Text = "До";
                    nMinProductCost = -1;
                    nMaxProductCost = -1;
                }
            }

            if (bShowEmployeeControl)
            {
                EmployeeControl.Visibility = Visibility.Visible;

                if(typeOfUser == TypeOfUser.Admin)
                {
                    DGEmployee.CanUserAddRows = (!bShowSearchEmployee && !bShowFilterEmployee);
                    DGEmployee.CanUserDeleteRows = !bShowSearchEmployee && !bShowFilterEmployee;
                    DGEmployee.IsReadOnly = bShowSearchEmployee || bShowFilterEmployee;
                }
                else
                {
                    DGEmployee.CanUserAddRows = false;
                    DGEmployee.CanUserDeleteRows = false;
                    DGEmployee.IsReadOnly = true;
                }

                SearchEmployee.Visibility = (bShowSearchEmployee ? Visibility.Visible : Visibility.Collapsed);
                if (!bShowSearchEmployee)
                {
                    SearchNameEmployee.Text = "ФИО";
                    strSearchEmployeeName = "";
                }


                FilterEmployee.Visibility = (bShowFilterEmployee ? Visibility.Visible : Visibility.Collapsed);
                if(!bShowFilterEmployee)
                {
                    FilterExpFrom.Text = "От";
                    FilterExpTo.Text = "До";
                    nMinEmployeeExperience = -1;
                    nMaxEmployeeExperience = -1;
                }    
                
            }

            if (bShowUserControl)
            {
                UserControl.Visibility = Visibility.Visible;

                DGPUsers.CanUserDeleteRows = !bShowSearchUser && !bShowAddUser;
                BanUserColumn.IsReadOnly = bShowSearchUser || bShowAddUser;

                SearchUser.Visibility = (bShowSearchUser ? Visibility.Visible : Visibility.Collapsed);
                if (!bShowSearchUser)
                {
                    SearchLogin.Text = "Логин";
                    strSearchUserLogin = "";
                }

                AddUser.Visibility = (bShowAddUser ? Visibility.Visible : Visibility.Collapsed);
                if(!bShowAddUser)
                {
                    AddLogin.Text = "Логин";
                    AddPassword.Password = "";
                    AddType.SelectedItem = "Клиент";
                }
            }

            if (bShowExpertMethod)
            {
                ExpertMethod.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePage()
        {
            UpdateVisibility();

            if (bShowProductControl)  UpdateProduct();
            if (bShowEmployeeControl) UpdateEmployee();
            if (bShowUserControl)     UpdateUser();
        }

        private void UpdateProduct()
        {
            UpdateProductList();

            details.Clear();
            details = module.GetAllDetails();

            subdetails.Clear();

            foreach(var i in details)
            {
                bool bShow = true;

                if (strSearchProductName.Length != 0) bShow &= i.Name.Contains(strSearchProductName);

                if (nMinProductCost != -1) bShow &= i.Cost >= nMinProductCost;
                if (nMaxProductCost != -1) bShow &= i.Cost <= nMaxProductCost;

                if(bShow) subdetails.Add(i);
            }
            DGProduct.ItemsSource = null;
            DGProduct.ItemsSource = subdetails;
        }

        private void UpdateEmployee()
        {
            UpdateEmployeeList();

            creditors.Clear();
            creditors = module.GetAllCreditors();

            subcreditors.Clear();

            foreach(var i in creditors)
            {
/*                bool bShow = true;

                if (strSearchEmployeeName.Length != 0) bShow &= i.FullName.Contains(strSearchEmployeeName);
                if (strSearchEmployeePosition.Length != 0)
                {
                    if (strSearchEmployeePosition.Equals("Уборщик")) bShow &= i.Position == Position.Cleaner;
                    if (strSearchEmployeePosition.Equals("Пекарь")) bShow &= i.Position == Position.Baker;
                    if (strSearchEmployeePosition.Equals("Менеджер")) bShow &= i.Position == Position.Manager;
                    if (strSearchEmployeePosition.Equals("Менеджер по продажам")) bShow &= i.Position == Position.SalesManager;
                    if (strSearchEmployeePosition.Equals("Администратор")) bShow &= i.Position == Position.Administrator;
                }

                if (nMinEmployeeExperience != -1) bShow &= i.Experience >= nMinEmployeeExperience;
                if (nMaxEmployeeExperience != -1) bShow &= i.Experience <= nMaxEmployeeExperience;

                if (bShow) subcreditors.Add(i);*/
            }

            //DGEmployee.ItemsSource = null;
            //DGEmployee.ItemsSource = subcreditors;

            //PositionCombo.ItemsSource = null;
            //PositionCombo.ItemsSource = positions;
        }

        private void UpdateUser()
        {
            UpdateUserList();

            clients.Clear();
            clients = module.GetAllClients();

            admins.Clear();
            admins = module.GetAllAdmins();

            experts.Clear();
            experts = module.GetAllExperts();

            userItems.Clear();

            foreach(var i in admins)
            {
                bool bShow = true;

                if (strSearchUserLogin.Length != 0) bShow &= i.Login.Contains(strSearchUserLogin);

                if (bShow) userItems.Add(new UserItem(i));
            }

            foreach (var i in experts)
            {
                bool bShow = true;

                if (strSearchUserLogin.Length != 0) bShow &= i.Login.Contains(strSearchUserLogin);

                if (bShow) userItems.Add(new UserItem(i));
            }

            foreach (var i in clients)
            {
                bool bShow = true;

                if (strSearchUserLogin.Length != 0) bShow &= i.Login.Contains(strSearchUserLogin);

                if (bShow) userItems.Add(new UserItem(i));
            }

            DGPUsers.ItemsSource = null;
            DGPUsers.ItemsSource = userItems;
        }

        private void UpdateProductList()
        {
            foreach (var i in subdetails)
            {
                switch (module.ModifyDetail(i))
                {
                    case ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Error:
                        {
                            module.CreateDetail(i);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private void UpdateEmployeeList()
        {
            foreach (var i in subcreditors)
            {
                switch (module.ModifyCreditor(i))
                {
                    case ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Error:
                        {
                            module.CreateCreditor(i);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private void UpdateUserList()
        {
            foreach (var i in userItems)
            {
                if(i.Status.Equals("Админ"))
                {
                    foreach(var j in admins)
                    {
                        if(j.Login.Equals(i.Login))
                        {
                            if((j.UserStatus == Status.Banned) != i.IsBanned)
                            {
                                //Do nothing
                            }
                            break;
                        }
                    }
                }
                if (i.Status.Equals("Эксперт"))
                {
                    foreach (var j in experts)
                    {
                        if (j.Login.Equals(i.Login))
                        {
                            if ((j.UserStatus == Status.Banned) != i.IsBanned)
                            {
                                switch(j.UserStatus)
                                {
                                    case Status.Banned:
                                        {
                                            j.UserStatus = Status.NotBanned;
                                            var answer = module.ModifyExpert(j);
                                            break;
                                        }
                                    case Status.NotBanned:
                                        {
                                            j.UserStatus = Status.Banned;
                                            var answer = module.ModifyExpert(j);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    }
                }
                if (i.Status.Equals("Клиент"))
                {
                    foreach (var j in clients)
                    {
                        if (j.Login.Equals(i.Login))
                        {
                            if ((j.UserStatus == Status.Banned) != i.IsBanned)
                            {
                                switch (j.UserStatus)
                                {
                                    case Status.Banned:
                                        {
                                            j.UserStatus = Status.NotBanned;
                                            var answer = module.ModifyClient(j);
                                            break;
                                        }
                                    case Status.NotBanned:
                                        {
                                            j.UserStatus = Status.Banned;
                                            var answer = module.ModifyClient(j);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        // Отображение управления продуктами
        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            bShowProductControl = true;
            bShowEmployeeControl = false;
            bShowUserControl = false;
            bShowExpertMethod = false;

            UpdatePage();
        }

        private void BtnEmployee_Click(object sender, RoutedEventArgs e)
        {
            bShowProductControl = false;
            bShowEmployeeControl = true;
            bShowUserControl = false;
            bShowExpertMethod = false;

            UpdatePage();
        }

        // Отображение управления пользователями
        private void BtnUser_Click(object sender, RoutedEventArgs e)
        {
            bShowProductControl = false;
            bShowEmployeeControl = false;
            bShowUserControl = true;
            bShowExpertMethod = false;

            UpdatePage();
        }

        // Отображение экспертного метода
        private void BtnExpert_Click(object sender, RoutedEventArgs e)
        {
            bShowProductControl = false;
            bShowEmployeeControl = false;
            bShowUserControl = false;
            bShowExpertMethod = true;

            Z1.Text = "";
            Z2.Text = "";
            Z3.Text = "";
            Z4.Text = "";
            Z5.Text = "";

            R1.Text = "";

            UpdatePage();
        }

        // Создание отчета
        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter streamWriter = new StreamWriter("reportFile.txt", false))
            {
                streamWriter.Write(module.GetReport());
                MessageBox.Show("Отчёт создан!");
            }
        }

        // Выход из прилоги
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            UpdateProductList();
            UpdateEmployeeList();
            UpdateUserList();

            module.PreviousRoom();
            OnStartSession();
        }

        // Управление продуктами -> CrUD + View логика
        private void BtnViewProduct_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchProduct = false;
            bShowFilterProduct = false;

            UpdatePage();
        }

        // Управление продуктами -> Поиск
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchProduct = true;
            bShowFilterProduct = false;

            SearchName.Text = "Название";
            strSearchProductName = "";

            UpdatePage();
        }

        // Управление продуктами -> Фильтрация
        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchProduct = false;
            bShowFilterProduct = true;

            FilterFirstFrom.Text = "От";
            FilterFirstTo.Text = "До";
            nMinProductCost = -1;
            nMaxProductCost = -1;

            UpdatePage();
        }

        private void BtnViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchEmployee = false;
            bShowFilterEmployee = false;

            UpdatePage();
        }

        private void BtnSearchEmployee_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchEmployee = true;
            bShowFilterEmployee = false;

            SearchNameEmployee.Text = "ФИО";
            strSearchEmployeeName = "";

            UpdatePage();
        }

        private void BtnFilterEmployee_Click(object sender, RoutedEventArgs e)
        {
            bShowSearchEmployee = false;
            bShowFilterEmployee = true;

            FilterExpFrom.Text = "От";
            FilterExpTo.Text = "До";
            nMinEmployeeExperience = -1;
            nMaxEmployeeExperience = -1;

            UpdatePage();
        }

        private void BtnViewUser_Click(object sender, RoutedEventArgs e)
        {
            bShowAddUser = false;
            bShowSearchUser = false;

            UpdatePage();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            bShowAddUser = true;
            bShowSearchUser = false;

            AddLogin.Text = "Логин";
            AddPassword.Password = "";
            AddType.SelectedItem = "Клиент";

            UpdatePage();
        }

        private void BtnSearchUser_Click(object sender, RoutedEventArgs e)
        {
            bShowAddUser = false;
            bShowSearchUser = true;

            SearchLogin.Text = "Логин";
            strSearchUserLogin = "";

            UpdatePage();
        }
 
        private void BtnApplySearchUser_Click(object sender, RoutedEventArgs e)
        {
            strSearchUserLogin = SearchLogin.Text;
            if (strSearchUserLogin.Equals("Логин")) strSearchUserLogin = "";

            UpdatePage();
        }

        private void DGProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if(DGProduct.CanUserDeleteRows)
                {
                    Detail obj;
                    try
                    {
                        obj = DGProduct.SelectedItem as Detail;
                        var answer = module.DeleteDetail(obj.Id);
                        subdetails.Remove(obj);
                        if (answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Error)
                        {
                            string messageBoxText = "Ошибка!";
                            string caption = "Ошибка";
                            MessageBoxResult result;

                            result = MessageBox.Show(messageBoxText, caption);
                        }
                    }
                    catch (Exception)
                    {
                        string messageBoxText = "Критическая ошибка!";
                        string caption = "Ошибка";
                        MessageBoxResult result;

                        result = MessageBox.Show(messageBoxText, caption);
                        return;
                    }
                    DGProduct.ItemsSource = null;
                    DGProduct.ItemsSource = subdetails;
                }
            }
        }

        private void DGProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!bShowSearchProduct && !bShowFilterProduct)
            {
                if(bEditedProduct)
                {
                    UpdateProduct();
                    bEditedProduct = false;
                }
            }
        }

        private void DGProduct_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bEditedProduct = true;
        }

        private void DGEmployee_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DGEmployee.CanUserDeleteRows)
                {
                    Creditor obj;
                    try
                    {
                        obj = DGEmployee.SelectedItem as Creditor;
                        var answer = module.DeleteCreditor(obj.Id);
                        subcreditors.Remove(obj);
                        if (answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Error)
                        {
                            string messageBoxText = "Ошибка!";
                            string caption = "Ошибка";
                            MessageBoxResult result;

                            result = MessageBox.Show(messageBoxText, caption);
                        }
                    }
                    catch (Exception)
                    {
                        string messageBoxText = "Критическая ошибка!";
                        string caption = "Ошибка";
                        MessageBoxResult result;

                        result = MessageBox.Show(messageBoxText, caption);
                        return;
                    }
                    DGEmployee.ItemsSource = null;
                    DGEmployee.ItemsSource = subcreditors;
                }
            }
        }

        private void DGEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!bShowSearchEmployee && !bShowFilterEmployee)
            {
                if (DGEmployee.SelectedIndex == selEmployeeIndex) return;
                selEmployeeIndex = DGEmployee.SelectedIndex;

                if (bEditedEmployee)
                {
                    UpdateEmployee();
                    bEditedEmployee = false;
                }
            }
        }

        private void DGEmployee_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bEditedEmployee = true;
        }

        private void DGPUsers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DGPUsers.CanUserDeleteRows)
                {
                    UserItem obj;
                    try
                    {
                        obj = DGPUsers.SelectedItem as UserItem;

                        foreach(var i in userItems)
                        {
                            if(i.Login.Equals(obj.Login))
                            {
                                ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer answer = ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.UnknownCommand;

                                if (obj.Status.Equals("Админ"))
                                {
                                    string messageBoxText = "Админа удалить нельзя";
                                    string caption = "Ошибка";
                                    MessageBoxResult result;

                                    result = MessageBox.Show(messageBoxText, caption);
                                    UpdateUser();
                                    return;
                                }
                                if (obj.Status.Equals("Эксперт"))
                                {
                                    answer = module.DeleteExpertWith(obj.Login);
                                }
                                if (obj.Status.Equals("Клиент"))
                                {
                                    answer = module.DeleteClientWith(obj.Login);
                                }

                                userItems.Remove(obj);

                                if (answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Error)
                                {
                                    string messageBoxText = "Ошибка!";
                                    string caption = "Ошибка";
                                    MessageBoxResult result;

                                    result = MessageBox.Show(messageBoxText, caption);
                                }

                                break;
                            }
                        }

                        
                    }
                    catch (Exception)
                    {
                        string messageBoxText = "Критическая ошибка!";
                        string caption = "Ошибка";
                        MessageBoxResult result;

                        result = MessageBox.Show(messageBoxText, caption);
                        return;
                    }
                    DGPUsers.ItemsSource = null;
                    DGPUsers.ItemsSource = userItems;
                }
            }
        }

        private void DGPUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (!bShowSearchUser && !bShowAddUser)
            //{
            //    if (DGPUsers.SelectedIndex == selUserIndex) return;
            //    selUserIndex = DGPUsers.SelectedIndex;

            //    if (bEditedUser)
            //    {
            //        UpdateUser();
            //        bEditedUser = false;
            //    }
            //}
        }

        private void DGPUsers_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bEditedUser = true;
        }

        private void DGPUsers_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (!bShowSearchUser && !bShowAddUser)
            //{
            //    if (bEditedUser)
            //    {
            //        UpdateUser();
            //        bEditedUser = false;
            //    }
            //}
        }

        private void BtnApplySearch_Click(object sender, RoutedEventArgs e)
        {
            strSearchProductName = SearchName.Text;
            if (strSearchProductName.Equals("Название")) strSearchProductName = "";

            UpdatePage();
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            String str = FilterFirstFrom.Text;

            if (!str.Equals("От") && !str.Equals(""))
            {
                if (!Int32.TryParse(str, out nMinProductCost))
                {
                    string messageBoxText = "Некорректный ввод!";
                    string caption = "Ошибка";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                    FilterFirstFrom.Text = "От";
                    return;
                }
            }
            else nMinProductCost = -1;

            str = FilterFirstTo.Text;

            if (!str.Equals("До") && !str.Equals(""))
            {
                if (!Int32.TryParse(str, out nMaxProductCost))
                {
                    string messageBoxText = "Некорректный ввод!";
                    string caption = "Ошибка";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                    FilterFirstTo.Text = "До";
                    return;
                }
            }
            else nMaxProductCost = -1;

            UpdatePage();
        }

        private void BtnPayEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (DGEmployee.SelectedIndex != -1)
            {
                var employee = DGEmployee.SelectedItem as Creditor;
            }
        }

        private void BtnApplySearchEmployee_Click(object sender, RoutedEventArgs e)
        {
            strSearchEmployeeName = SearchNameEmployee.Text;
            if (strSearchEmployeeName.Equals("ФИО")) strSearchEmployeeName = "";

            UpdatePage();
        }

        private void BtnApplyFilterEmployee_Click(object sender, RoutedEventArgs e)
        {
            String str = FilterExpFrom.Text;

            if (!str.Equals("От") && !str.Equals(""))
            {
                if (!Int32.TryParse(str, out nMinEmployeeExperience))
                {
                    string messageBoxText = "Некорректный ввод!";
                    string caption = "Ошибка";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                    FilterExpFrom.Text = "От";
                    return;
                }
            }
            else nMinEmployeeExperience = -1;

            str = FilterExpTo.Text;

            if (!str.Equals("До") && !str.Equals(""))
            {
                if (!Int32.TryParse(str, out nMaxEmployeeExperience))
                {
                    string messageBoxText = "Некорректный ввод!";
                    string caption = "Ошибка";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                    FilterExpTo.Text = "До";
                    return;
                }
            }
            else nMaxEmployeeExperience = -1;

            UpdatePage();
        }

        private void BtnApplyAddUser_Click(object sender, RoutedEventArgs e)
        {
            UpdateUser();
            String login = AddLogin.Text;
            String password = AddPassword.Password;
            if(login.Length==0 || password.Length==0)
            {
                string messageBoxText = "Поля не должны быть пустыми";
                string caption = "Ошибка";
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption);
                return;
            }

            bool bFound = false;

            foreach(var i in admins)
            {
                if (i.Login.Equals(login)) bFound = true;
            }
            foreach (var i in experts)
            {
                if (i.Login.Equals(login)) bFound = true;
            }
            foreach (var i in clients)
            {
                if (i.Login.Equals(login)) bFound = true;
            }

            if(bFound)
            {
                string messageBoxText = "Аккаунт с таким логином уже существует";
                string caption = "Ошибка";
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption);
                return;
            }

            if(AddType.Text.Equals("Админ"))
            {
                var answer = module.RegisterNewAdmin(new Admin(login, password));

                if(answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Successfully)
                {
                    string messageBoxText = "Успешно";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
                else
                {
                    string messageBoxText = "Произошла ошибка сервера";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
            }
            if (AddType.Text.Equals("Эксперт"))
            {
                var answer = module.RegisterNewExpert(new Expert(login, password));

                if (answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Successfully)
                {
                    string messageBoxText = "Успешно";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
                else
                {
                    string messageBoxText = "Произошла ошибка сервера";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
            }
            if (AddType.Text.Equals("Клиент"))
            {
                var answer = module.RegisterNewClient(new Client(login, password));

                if (answer == ClassLibraryForTCPConnectionAPI_C_sharp_.AnswerFromServer.Successfully)
                {
                    string messageBoxText = "Успешно";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
                else
                {
                    string messageBoxText = "Произошла ошибка сервера";
                    string caption = "";
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption);
                }
            }

            UpdateUser();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            

            
            
            {

                int rz1 = 0;
                int rz2 = 0;
                int rz3 = 0;
                int rz4 = 0;
                int rz5 = 0;
                int rz11 = 0;
                int rz12 = 0;
                int rz13 = 0;
                int rz14 = 0;
                int rz15 = 0;

                if (!Int32.TryParse(Z1.Text,out rz1) || !Int32.TryParse(Z2.Text, out rz2) || !Int32.TryParse(Z3.Text, out rz3) || !Int32.TryParse(Z4.Text, out rz4) || !Int32.TryParse(Z5.Text, out rz5))
                {
                    MessageBox.Show("Некорректный ввод!");
                    return;
                }
                if (!Int32.TryParse(Z11.Text, out rz11) || !Int32.TryParse(Z12.Text, out rz12) || !Int32.TryParse(Z13.Text, out rz13) || !Int32.TryParse(Z14.Text, out rz14) || !Int32.TryParse(Z15.Text, out rz15))
                {
                    MessageBox.Show("Некорректный ввод!");
                    return;
                }
                if (R1.Text == "0" || (rz1 + rz2 + rz3 + rz4 + rz5) != 100)
                {
                    MessageBox.Show("Необходимо заполнить рейтинг!\nСумма полей оценки должно быть рано 100!");
                    return;
                }
                else if (R11.Text == "0" || (rz11 + rz12 + rz13 + rz14 + rz15) != 100)
                {
                    MessageBox.Show("Необходимо заполнить рейтинг!\nСумма полей оценки должно быть рано 100!");
                    return;
                }

                int rt1=10;
                int rt11=10;
                if (!Int32.TryParse(R1.Text, out rt1)||!Int32.TryParse(R11.Text,out rt11))
                {
                    MessageBox.Show("Некорректный ввод");
                    return;
                }
                

                double s1, s2, s3, s4, s5;

                s1 = rt1 * rz1+rt11*rz11;
                s2 = rt1 * rz2 + rt11 * rz12;
                s3 = rt1 * rz3 + rt11 * rz13;
                s4 = rt1 * rz4 + rt11 * rz14;
                s5 = rt1 * rz5 + rt11 * rz15;

                double sum = s1 + s2 + s3 + s4 + s5;

                double w1, w2, w3, w4, w5;

                w1 = Math.Round(s1 / sum, 2);
                w2 = Math.Round(s2 / sum, 2);
                w3 = Math.Round(s3 / sum, 2);
                w4 = Math.Round(s4 / sum, 2);
                w5 = Math.Round(s5 / sum, 2);

                Rez_Z1.Text = w1.ToString();
                Rez_Z2.Text = w2.ToString();
                Rez_Z3.Text = w3.ToString();
                Rez_Z4.Text = w4.ToString();
                Rez_Z5.Text = w5.ToString();

                var map = new Dictionary<string, double>();
                map.Add("1", w1);
                map.Add("2", w2);
                map.Add("3", w3);
                map.Add("4", w4);
                map.Add("5", w5);

                MessageBox.Show("Лучший вариант:  " + map.OrderByDescending(key => key.Value).First().Key + "  с коэффициентом  " + map.OrderByDescending(key => key.Value).First().Value.ToString());
            }
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {

            Z1.Text = "";
            Z2.Text = "";
            Z3.Text = "";
            Z4.Text = "";
            Z5.Text = "";

            R1.Text = "";

            Z11.Text = "";
            Z12.Text = "";
            Z13.Text = "";
            Z14.Text = "";
            Z15.Text = "";

            R11.Text = "";
            Rez_Z1.Text = "";
            Rez_Z2.Text = "";
            Rez_Z3.Text = "";
            Rez_Z4.Text = "";
            Rez_Z5.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateProductList();
            UpdateEmployeeList();
            UpdateUserList();
        }
    }
}
