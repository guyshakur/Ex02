using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWinFormsApp.WeatherFeature;
using FacebookWinFormsApp.FinanceFeature;
using FacebookWinFormsApp.CostumText;
using WPFCustomMessageBox;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Threading;

namespace FacebookWinFormsApp
{
    public partial class MainForm : Form
    {
        public LoginFacade LoginFacade { get; set; }
        private readonly List<object> r_LastPostsCollection = new List<object>();

        public MainForm(User m_LoginUser)
        {
            //r_LoggedUser = m_LoginUser;
            //InitializeComponent();
            //fetchLoginDetails();
            //new Thread(fetchLoginDetails).Start();
            LoginFacade = new LoginFacade();
            LoginFacade.LoginUser = m_LoginUser;
               
        }
        

        public CustomText m_customText { get; set; }


        protected override void OnLoad(EventArgs e)
        {
            InitializeComponent();
            fillCustomPostsBoxFromFile();
            fetchLoginDetails();
        }

       


        private void fetchLoginDetails()
        {
            pictureBoxProfile.Invoke(new Action(() => pictureBoxProfile.ImageLocation = LoginFacade.LoginUser.PictureLargeURL));
            Text = $"{LoginFacade.LoginUser.FirstName} {LoginFacade.LoginUser.LastName}";
            new Thread(fetchSelfDetails).Start();

            try
            {
                fetchWeatherDetails(LoginFacade.LoginUser.Location.Name);
               //fetchWeatherDetails(r_LoggedUser.Location.Name);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                System.Windows.MessageBox.Show("Can't fetch weather details");
            }
        }

        private void fetchWeatherDetails(string i_CityLocation)
        {
            WeatherDetails weatherDetails = WeatherFeature.WeatherFeature.GetWeatherDetails(i_CityLocation);
            labelCountry.Text = $"Country: {weatherDetails.Location.Country}";
            labelCountry.Visible = true;
            labelLastUpdate.Text = $"Last Update: {weatherDetails.Location.LocalTime}";
            labelLastUpdate.Visible = true;
            labelCity.Text = $"City: {weatherDetails.Location.City}";
            labelCity.Visible = true;
            labelPredictWeather.Text = $"Predict: {weatherDetails.DailyPredict.Condition.PredictText}";
            labelPredictWeather.Visible = true;
            labelTemperatureInCelcius.Text = $"Temperture in Celcius: {weatherDetails.DailyPredict.TempertureInCelsius}";
            labelTemperatureInCelcius.Visible = true;
            labelTemperatureInFahrnheit.Text = $"Temperture in Fahrenheit: {weatherDetails.DailyPredict.TempertureInFahrenheit}";
            labelTemperatureInFahrnheit.Visible = true;
            pictureBoxWeatherPredict.ImageLocation = "Http:" + weatherDetails.DailyPredict.Condition.PredictIcon;
            pictureBoxWeatherPredict.Visible = true;
            buttonFetchWeatherDetails.Visible = true;
            labelWeatherDetails.Visible = true;
        }

        private void fetchSelfDetails()
        {
            labelFirstName.Invoke(new Action(() => labelFirstName.Text += LoginFacade.LoginUser.FirstName));
            labelLastName.Invoke(new Action(() => labelLastName.Text += LoginFacade.LoginUser.LastName));
            labelEmail.Invoke(new Action(() => labelEmail.Text += LoginFacade.LoginUser.Email));
            labelGender.Invoke(new Action(() => labelGender.Text += LoginFacade.LoginUser.Gender.ToString()));
            labelBirthday.Invoke(new Action(() => labelBirthday.Text += LoginFacade.LoginUser.Birthday));
            
        }

        private void fetchLikedPages()
        {
            //listBoxLikedPages.Items.Clear();
            listBoxLikedPages.Invoke(new Action(() => listBoxLikedPages.Items.Clear()));

            try
            {
                foreach (Page page in LoginFacade.LoginUser.LikedPages)
                {
                    //listBoxLikedPages.Items.Add(page.Name);
                    listBoxLikedPages.Invoke(new Action(() => listBoxLikedPages.Items.Add(page.Name)));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (listBoxLikedPages.Items.Count == 0)
            {
                listBoxLikedPages.Invoke(new Action(() => listBoxLikedPages.Items.Add("There are no liked pages for this user")));
                //listBoxLikedPages.Items.Add("There are no liked pages for this user");
            }
        }

        
        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            LoginFacade.LogOut();
            LoginPageForm formLoginPage = new LoginPageForm();
            Hide();
            formLoginPage.ShowDialog();
            Close();
        }

        private void buttonLikedPages_Click(object sender, EventArgs e)
        {
            new Thread(fetchLikedPages).Start();
        }

        private void buttonFetchPosts_Click(object sender, EventArgs e)
        {
            new Thread(fetchPosts).Start();
        }

        private void fetchPosts()
        {
            LoginFacade.LoginUser.ReFetch();
            //listBoxPosts.Items.Clear();
            listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Clear()));
            // listBoxComments.Invoke(new Action(() => listBoxComments.Items.Clear()));
            //listBoxComments.Items.Clear();
            listBoxPosts.Invoke(new Action(() => listBoxPosts.DisplayMember = "Message"));
            //listBoxPosts.DisplayMember = "Message";

            try
            {
                foreach (Post post in LoginFacade.LoginUser.Posts)
                {
                    if (post.Message != null)
                    {
                        //listBoxPosts.Items.Add(post);
                        listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Add(post)));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (LoginFacade.LoginUser.Posts.Count == 0)
            {
                System.Windows.MessageBox.Show("No Posts to retrieve.");
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            searchInPost(listBoxPosts.Text);
        }

        private void searchInPost(string i_StringToSearch)
        {
            if (listBoxPosts.Items.Count != 0)
            {
                int id = listBoxPosts.FindString(i_StringToSearch);

                if (id >= 0)
                {
                    listBoxPosts.SetSelected(id, true);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please fetch posts before");
            }
        }

        private void listBoxPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            Post selected = LoginFacade.LoginUser.Posts[listBoxPosts.SelectedIndex];
            listBoxComments.DisplayMember = "Message";
            listBoxComments.DataSource = selected.Comments;
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            searchInPost((sender as TextBox)?.Text);
        }

        private void checkBoxSortPostsByOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (listBoxPosts.Items.Count != 0 && (sender as CheckBox)?.Checked == true)
            {
                try
                {
                    r_LastPostsCollection.Clear();
                    foreach (Post post in listBoxPosts.Items)
                    {
                        r_LastPostsCollection.Add(post);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }

                listBoxPosts.Sorted = true;
            }
            else if (listBoxPosts.Items.Count == 0)
            {
                System.Windows.MessageBox.Show("no posts");
            }
            else
            {
                listBoxPosts.Sorted = false;
                listBoxPosts.Items.Clear();
                foreach (object obj in r_LastPostsCollection)
                {
                    listBoxPosts.Items.Add(obj);
                }
            }
        }

        private void listBoxLikedPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page selected = LoginFacade.LoginUser.LikedPages[listBoxLikedPages.SelectedIndex];
            new Thread(()=> webBrowserPages.Navigate(selected.URL)).Start();
            
        }

        private void listBoxPhotos_SelectedIndexChanged(object sender, EventArgs e)
        {
            new Thread(listBoxPhotosSelectedPhoto).Start();
            
        }

        private void listBoxPhotosSelectedPhoto()
        {
            if (listBoxPhotos.SelectedItem != null)
            {
                Photo selectedPhoto = listBoxPhotos.SelectedItem as Photo;
                if (selectedPhoto != null)
                {
                    
                    pictureBoxPhoto.ImageLocation = selectedPhoto.PictureNormalURL;
                    pictureBoxPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    listBoxPhotosComments.Invoke(new Action( ()=>listBoxPhotosComments.DataSource = selectedPhoto.Comments));
                }
            }
        }

        private void buttonFetchAlbums_Click(object sender, EventArgs e)
        {
            new Thread(fetchAlbums).Start();
        }

        private void fetchAlbums()
        {
            //listBoxAlbums.Items.Clear();
            //listBoxAlbums.DisplayMember = "Name";
            listBoxAlbums.Invoke(new Action(() => listBoxAlbums.Items.Clear()));
            listBoxAlbums.Invoke(new Action(() => listBoxAlbums.DisplayMember = "Name"));

            try
            {
                foreach (Album album in LoginFacade.LoginUser.Albums)
                {
                    //listBoxAlbums.Items.Add(album);
                    listBoxAlbums.Invoke(new Action(() => listBoxAlbums.Items.Add(album)));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (listBoxAlbums.Items.Count == 0)
            {
                System.Windows.MessageBox.Show("No Albums to retrieve :(");
            }
        }

        private void listBoxAlbums_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxPhotos.Items.Clear();       
            listBoxPhotos.DisplayMember = "CreatedTime.ToString()";
            Album albumSelected = listBoxAlbums.SelectedItem as Album;

            try
            {
                foreach (Photo photo in albumSelected.Photos)
                {
                    listBoxPhotos.Items.Add(photo);
                    //listBoxPhotos.Invoke(new Action(() => listBoxPhotos.Items.Add(photo)));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (listBoxPhotos.Items.Count == 0)
            {
                MessageBox.Show("No photos to fetch");
            }
        }

        private void buttonLike_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxPhotos.SelectedItem != null)
                {
                    Photo photo = listBoxPhotos.SelectedItem as Photo;
                    if (!photo.LikedBy.Contains(LoginFacade.LoginUser))
                    {
                        photo.Like();
                    }
                    else
                    {
                        photo.Unlike();
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("problem has been deteced with liking post. Contact FaceBook administrator.");
            }
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            postMessage();
        }

        private void postMessage()
        {
            try
            {
                LoginFacade.LoginUser.PostStatus(textBoxPost.Text);
                System.Windows.MessageBox.Show("Your post shared sucessfully");
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Your post can't be shared. Please contact facebook administrator ");
            }
            finally
            {
                textBoxPost.Text = string.Empty;
            }
        }

        private void listBoxFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            User friend = listBoxFriends.SelectedItem as User;
            new Thread( ()=>fetchFriendsDetails(friend)).Start();
        }

        private void fetchFriendsDetails(User i_Friend)
        {
            if (i_Friend != null)
            {
                labelFriendFirstName.Text = $"First Name: {i_Friend.FirstName}";
                labelFriendLastName.Text = $"Last Name: {i_Friend.LastName}";
                labelFriendEmail.Text = $"Email: {i_Friend.Email}";
                labelFriendGender.Text = $"Gender: {i_Friend.Gender.ToString()}";
                labelFriendBirthday.Text = $"Birthday: {i_Friend.Birthday}";
                pictureBoxFriend.ImageLocation = i_Friend.PictureLargeURL;
            }
        }

        private void buttonFetchWeatherDetails_Click(object sender, EventArgs e)
        {
            try
            {
                fetchWeatherDetails(LoginFacade.LoginUser.Location.Name);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Can't fetch weather details");
            }
        }

        private void buttonFetchStockDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Stock searchStock = FinanceFeature.FinanceFeature.GetStocksDetails(textBoxSearchStock.Text);
                labelStockPrice.Text = $"Price: {searchStock.Price.ToString()}";
                labelStockIpo.Text = $"Stock Ipo: {searchStock.IpoDate}";
                labelStockChanges.Text = $"Changes: {searchStock.Changes.ToString()}";
                pictureBoxStock.ImageLocation = searchStock.Image;
            }
            catch
            {
                System.Windows.MessageBox.Show("Invalid Stock Name");
            }
        }

        private void buttonSaveCustomPostToList_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxCustomPost.Text))
            {
                m_customText.createMessageAndAddToList(textBoxCustomPost.Text);
                listBoxCustomPosts.Items.Add(m_customText.TextMessage.ElementAt(m_customText.TextMessage.Count - 1));
                m_customText.SaveToFile();
                textBoxCustomPost.Clear();
            }
        }

        private void buttonClearText_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxCustomPost.Text))
            {
                textBoxCustomPost.Clear();
            }
        }

        private void buttonRemoveAllCustomPosts_Click(object sender, EventArgs e)
        {
            if (m_customText != null)
            {
                if (m_customText.TextMessage.Count != 0 && listBoxCustomPosts != null)
                {
                    listBoxCustomPosts.Items.Clear();
                    m_customText.ClearMessages();
                    m_customText.SaveToFile();
                }
            }
        }

        private void buttonRemoveCustomPost_Click(object sender, EventArgs e)
        {
            if (m_customText != null)
            {
                if (m_customText.TextMessage.Count != 0 && listBoxCustomPosts.SelectedItem != null)
                {
                    m_customText.RemoveMessageFromList(listBoxCustomPosts.SelectedIndex);
                    listBoxCustomPosts.Items.Remove(listBoxCustomPosts.SelectedItem);
                    m_customText.SaveToFile();
                }
            }
        }

        private void fillCustomPostsBoxFromFile()
        {
            m_customText = CustomText.CustomTextInstance;
            try
            {
                m_customText = CustomText.LoadFile();

                foreach (string message in m_customText.TextMessage)
                {
                    listBoxCustomPosts.Items.Add(message);
                }
            }
            catch (Exception ex)
            {
                //m_customText.SaveToFile();
                throw new Exception(ex.Message);
            }
        }

        private void buttonEditCustomPost_Click(object sender, EventArgs e)
        {
            editPost();
        }

        private void editPost()
        {
            if (m_customText != null)
            {
                if (m_customText.TextMessage.Count != 0 && listBoxCustomPosts.SelectedItem != null)
                {
                    int index = listBoxCustomPosts.SelectedIndex;
                    textBoxCustomPost.Text = listBoxCustomPosts.SelectedItem.ToString();
                    textBoxCustomPost.Focus();
                    m_customText.RemoveMessageFromList(index);
                    listBoxCustomPosts.Items.RemoveAt(index);
                    m_customText.SaveToFile();
                }
            }
        }

        private void buttonFetchFriends_Click(object sender, EventArgs e)
        {
            if (LoginFacade.LoginUser.Friends != null)
            {
                try
                {
                    foreach (User user in LoginFacade.LoginUser.Friends)
                    {
                        listBoxFriends.Items.Add(user);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }

                if (listBoxFriends.Items.Count == 0)
                {
                    MessageBox.Show("no Friends to fetch");
                }
            }
        }

        private void listBoxCustomPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxCustomPosts != null && listBoxCustomPosts.SelectedItem != null)
            {
                string post = listBoxCustomPosts.SelectedItem.ToString();
                MessageBoxResult result = CustomMessageBox.ShowOKCancel(post, "Custom Post", "Edit Template", "Use Template To post");

                if (result == MessageBoxResult.OK)
                {
                    editPost();
                    listBoxCustomPosts.ClearSelected();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    this.tabControl.SelectedTab = tabProfile;
                    this.textBoxPost.Text = post;
                    listBoxCustomPosts.ClearSelected();
                }
            }
        }

        private void buttonChooseCustomedPost_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabCustomPost;
        }

        private void listBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGroups.SelectedItem != null)
            {
                Group selectedGroup = listBoxGroups.SelectedItem as Group;
                pictureBoxGroups.LoadAsync(selectedGroup.PictureNormalURL);
            }
        }

        private void buttonFetchGroups_Click(object sender, EventArgs e)
        {
            listBoxGroups.Items.Clear();
            listBoxGroups.DisplayMember = "Name";
            try
            {
                foreach (Group group in LoginFacade.LoginUser.Groups)
                {
                    listBoxGroups.Items.Add(group);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (listBoxGroups.Items.Count == 0)
            {
                MessageBox.Show("No groups to fetch");
            }
        }
    }
}