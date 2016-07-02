using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace App10
{
    public sealed partial class PhonesListPage : Page
    {
        ObservableCollection<Phone> phones;
        List<Company> companies;
        public PhonesListPage()
        {
            this.InitializeComponent();

            this.Loaded += PhonesListPage_Loaded;
        }

        private void PhonesListPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (MobileContext db = new MobileContext())
            {
                phones = new ObservableCollection<Phone>(db.Phones.ToList());
                companies = db.Companies.ToList();
            }

            companiesList.ItemsSource = companies;
            phonesList.ItemsSource = phones;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Company company = companiesList.SelectedItem as Company;
            if (company == null) return;

            // создаем объект Phone
            Phone phone = new Phone
            {
                 Name = nameBox.Text,
                Price = Int32.Parse(priceBox.Text),
                Company = company,
                CompanyId = company.Id
            };

            using (MobileContext db = new MobileContext())
            {
                db.Phones.Add(phone);
                if (db.SaveChanges() > 0)
                    phones.Add(phone);
            }
        }
    }
}
