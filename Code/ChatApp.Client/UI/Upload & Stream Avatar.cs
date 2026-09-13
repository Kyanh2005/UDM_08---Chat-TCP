using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChatApp.Client.Models;

namespace ChatApp.Client.Controls
{
    public class ContactListControl : UserControl
    {
        private Label lblTitle;
        private FlowLayoutPanel contactPanel;

        private List<ContactModel> contacts;

        public event EventHandler<ContactModel> ContactSelected;

        public ContactListControl()
        {
            contacts = new List<ContactModel>();

            InitializeControl();
        }

        private void InitializeControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTitle = new Label();
            lblTitle.Text = "Contacts";
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 45;
            lblTitle.Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold
            );
            lblTitle.Padding = new Padding(15, 10, 0, 0);

            // Danh sách
            contactPanel = new FlowLayoutPanel();
            contactPanel.Dock = DockStyle.Fill;
            contactPanel.FlowDirection = FlowDirection.TopDown;
            contactPanel.WrapContents = false;
            contactPanel.AutoScroll = true;
            contactPanel.BackColor = Color.White;
            contactPanel.Padding = new Padding(5);

            this.Controls.Add(contactPanel);
            this.Controls.Add(lblTitle);
        }

        public void SetContacts(List<ContactModel> list)
        {
            contacts = list;

            RefreshContacts();
        }

        public void AddContact(ContactModel contact)
        {
            contacts.Add(contact);

            AddContactControl(contact);
        }

        public void RemoveContact(string userId)
        {
            for (int i = contacts.Count - 1; i >= 0; i--)
            {
                if (contacts[i].UserId == userId)
                {
                    contacts.RemoveAt(i);
                }
            }

            RefreshContacts();
        }

        public void UpdateContactStatus(
            string userId,
            bool isOnline)
        {
            foreach (ContactModel contact in contacts)
            {
                if (contact.UserId == userId)
                {
                    contact.IsOnline = isOnline;
                    break;
                }
            }

            RefreshContacts();
        }

        private void RefreshContacts()
        {
            contactPanel.Controls.Clear();

            foreach (ContactModel contact in contacts)
            {
                AddContactControl(contact);
            }
        }

        private void AddContactControl(ContactModel contact)
        {
            ContactItemControl item =
                new ContactItemControl(contact);

            item.Width =
                contactPanel.ClientSize.Width - 15;

            item.ContactClicked +=
                ContactItem_Clicked;

            contactPanel.Controls.Add(item);
        }

        private void ContactItem_Clicked(
            object sender,
            EventArgs e)
        {
            ContactItemControl item =
                sender as ContactItemControl;

            if (item == null)
                return;

            ContactSelected?.Invoke(
                this,
                item.Contact
            );
        }
    }
}
