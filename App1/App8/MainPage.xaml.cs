using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App8
{
    public class Phone
    {
        public int Id { get; set; }
        public string Title { get; set; } // модель телефона
        public string Company { get; set; } // производитель
    }
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Phone> Phones { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            Phones = new ObservableCollection<Phone>
            {
                new Phone {Id=1, Title="iPhone 6S", Company="Apple" },
                new Phone {Id=2, Title="Lumia 950", Company="Microsoft" },
                new Phone {Id=3, Title="Nexus 5X", Company="Google" },
            };
        }

        private async void phonesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Phone selectedPhone = (Phone)e.ClickedItem;
            await new Windows.UI.Popups.MessageDialog($"Выбран {selectedPhone.Title}").ShowAsync();
        }
        // обработчик кнопки
        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string title = titleTextBox.Text;
            string company = companyTextBox.Text;
            // добавление нового объекта
            Phones.Add(new Phone { Title = title, Company = company, Id = Phones.Count + 1 });
        }
    }
}
