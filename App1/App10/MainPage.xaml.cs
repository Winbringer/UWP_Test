using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
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

namespace App10
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        { 
            using (MobileContext db = new MobileContext())
            {
                companiesList.ItemsSource = db.Companies.ToList();
            }
        }
        private void PhonesList_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PhonesListPage));
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompanyPage));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделеный пункт меню
            if (companiesList.SelectedItem != null)
            {
                Company company = companiesList.SelectedItem as Company;
                if (company != null)
                    Frame.Navigate(typeof(CompanyPage), company.Id);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделеный пункт меню
            if (companiesList.SelectedItem != null)
            {
                Company company = companiesList.SelectedItem as Company;
                if (company != null)
                {
                    using (MobileContext db = new MobileContext())
                    {
                        db.Companies.Remove(company);
                        db.SaveChanges();
                        companiesList.ItemsSource = db.Companies.ToList();
                    }
                }
            }
        }
    }

    public class Phone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Phone> Phones { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public class MobileContext : DbContext
    {
        public MobileContext():base() { }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Phone> Phones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Mobile.db");
        }
    }
}
