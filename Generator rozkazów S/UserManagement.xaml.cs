using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;

namespace Generator_rozkazów_S;

public partial class UserManagement : Window
{
    private List<User> _users = null!;

    private DatabaseContext _dbCtx;
    private User _current = null!;
    private List<Role> _roles = null!;
    private User _userLoggedIn;
    private Role _userRole;
    public UserManagement(DatabaseContext dbCtx, User userLoggedIn)
    {
        InitializeComponent();
        _dbCtx = dbCtx;
        _userLoggedIn = userLoggedIn;
        User current = _dbCtx.Users.Include(x=>x.Role).First(x => x.Userid == userLoggedIn.Userid);
        if (current.Role is null)
        {
            MessageBox.Show("Błąd pobierania uprawnień", "Błąd uprawnień", MessageBoxButton.OK, MessageBoxImage.Error);
            throw new AuthenticationException();
        }
        _userRole = current.Role;
        _reload();
    }

    private Role? _getRole(User u)
    {
        return u.Role is null ? null : _roles.FirstOrDefault(role => role.Rolename == u.Role.Rolename);
    }

    private void _reload()
    {
        _users = _dbCtx.Users.Include(x => x.Role).ToList();
        _roles = _dbCtx.Role.ToList();
        Accounts.ItemsSource = _users;
        Accounts.SelectedIndex = 0;
        Roles.ItemsSource = _roles;
        Properties.DataContext = _current = _users[0];
    }

    private void Accounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(Accounts.SelectedItem is null) return;
        Properties.DataContext = _current = (User)Accounts.SelectedItem;
        Roles.SelectedItem = _getRole(_current);
    }

    private void Roles_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_userRole.Admin)
        {
            Role? current = _current.Role;
            Role next = (Role)Roles.SelectedItem;
            if (current is null && next.Admin)
            {
                MessageBox.Show("Nie masz uprawnień by nadać nowej osobie roli administracyjnej",
                    "Błąd uprawnień", MessageBoxButton.OK, MessageBoxImage.Error);
                Roles.SelectedItem = _getRole(_current);
                return;
            }
        }
        _current.Role = (Role)Roles.SelectedItem;
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        if (Password.Password.Length != 0 || RepeatPassword.Password.Length != 0)
        {
            if (Password.Password != RepeatPassword.Password)
            {
                Password.Background = RepeatPassword.Background = Brushes.LightCoral;
                MessageBox.Show("Hasła nie są takie same", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _current.Password = BCrypt.Net.BCrypt.HashPassword(Password.Password);
        }
        
        foreach (var user in _users)
        {
            bool failure = false;
            string reason = "";
            if (user.Password is null or "")
            {
                failure = true;
                reason = user.Username + " ma puste hasło";
            } else if (user.Role is null)
            {
                failure = true;
                reason = user.Username + " nie ma przypisanej roli";
            } else if (user.LastName is null or "")
            {
                failure = true;
                reason = user.Username + " ma puste nazwisko";
            }

            if (!failure) continue;
            MessageBox.Show(reason, "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        _dbCtx.SaveChanges();
        _reload();
        MessageBox.Show("Zapisano", "Zapisano", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (RepeatPassword.Password == Password.Password)
        {
            RepeatPassword.Background = Brushes.Transparent;
            Password.Background = Brushes.Transparent;
        }
        else
        {
            RepeatPassword.Background = Brushes.LightCoral;
            Password.Background = Brushes.LightCoral;
        }
    }

    private void Add_OnClick(object sender, RoutedEventArgs e)
    {
        User user = new User()
        {
            Username = "Nowy"
        };
        _users.Add(user);
        Accounts.Items.Refresh();
        Accounts.SelectedItem = user;
        _dbCtx.Users.Add(user);
    }

    private void Remove_OnClick(object sender, RoutedEventArgs e)
    {
        if (Accounts.SelectedItem is null) return;
        if (_users.First(x=> x.Role is not null && x.Role.Admin).Username == ((User)Accounts.SelectedItem).Username)
        {
            MessageBox.Show(
                "Nie możesz usunąć ostatniego użytkownika z uprawnieniami administratora",
                "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (((User)Accounts.SelectedItem).Userid == _userLoggedIn.Userid)
        {
            MessageBox.Show(
                "Nie możesz usunąć własnego konta", "Błąd usuwania",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Role? role = ((User)Accounts.SelectedItem).Role;
        if (role is { Admin: true } && !_userRole.Admin)
        {
            MessageBox.Show("Nie masz uprawnień do usunięcia tego konta", "Błąd uprawnień",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        _dbCtx.Users.Remove((User)Accounts.SelectedItem);
        _users.Remove((User)Accounts.SelectedItem);
        Accounts.SelectedIndex = 0;
        MessageBox.Show("Zmiany wejdą w życie po naciśnięciu przycisku Zapisz", "Informacja",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Refresh_OnClick(object sender, RoutedEventArgs e)
    {
        _reload();
    }
}