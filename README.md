<p align="center">
  <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/main_banner.png" alt="Title image" />
</p>

Welcome to the source code of **Profitocracy**. 

**Profitocracy** is a powerful budget management app designed to help users track their expenses effortlessly while following the 50-30-20 budgeting rule.

# Key Features of Profitocracy

- 💰 **Track Your Expenses**. Stay on top of your finances by organizing your spending effortlessly using the [50-30-20 rule](#503020rule).
- 📊 **Customize Spending Categories**. Create, set budgets, and easily monitor your spending in personalized categories.
- 📅 **Monthly Budget Planning**. Automatically generate a tailored budget to make every month stress-free.
- 🔒 **Complete Data Privacy**. Rest assured that your data is secure, Profitocracy does not share your information with third parties. Everything is stored locally on your device.
- 🌍 **Multi-Currency Support**. Track expenses in different currencies with seamless conversion for global users.
- 📈 **Visualize Your Spending with Charts**. Gain clear insights into your expenses with beautifully crafted charts and graphs.
- 👥 **Multiple Profiles**. Manage separate budgets or accounts for different individuals or purposes all within one app.
- 💻 **Open-Source**. Profitocracy is open-source, ensuring transparency and the support of a thriving community for constant improvements.

<p align="left">
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/transactions_banner.png" alt="Transactions" width="150"/>
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/categories_banner.png" alt="Categories" width="150"/>
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/multiprofiles_banner.png" alt="Multi-Profiles" width="150"/>
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/charts_banner.png" alt="Charts" width="150"/>
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/theme_banner.png" alt="Theme" width="150"/>
    <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/multicurrency_banner.png" alt="Multi-Currency" width="150"/>
</p>

## Supported Platforms

Profitocracy is created using .NET MAUI and can be launched on different platforms, but the main of them are:

- iOS;
- Android.

## <a name="503020rule"></a> What is 50-30-20 rule

The 50-30-20 rule is a common way to allocate the spending categories in your personal or household budget. 
The rule targets 50% of your after-tax income toward necessities, 30% toward things you don’t need—but make 
life a little nicer and the final 20% toward paying down debt and/or adding to your savings.

## Terminologies Used in Profitocracy

### 💼 Profile

Profile is an entity that tracks all of your expenses in a single place. 
It also calculates amounts for main, secondary and saving expenses, your 
every day expenses and expenses by categories.

### 🧾 Transaction

Transaction is a unit of moving funds. It could be an income (salary, for example) or expense (food, apartments) operation.
If it is expense operation your will need to specify the type of this expense - main, secondary or saving, - and its amount.
Optionally, you can specify also spending category (`None` by default), description and date of this transaction.

### 💵 Actual amount and planned amount of expenses

Almost everything that you can see at Home screen is an expense. Expense, in terms of Profitocracy, is an entity with
two values: *actual amount* and *plannedAmount*. Actual amount is your actual amount of spending of any type or category.
Planned amount is a planned by Profitocracy amount of money that you should not go beyond for every category or type.

### 📊 Category

Category is a special aggregation unit for your transactions. You can specify its name and planned amount for a month 
while creation process at **Settings** screen. Then you will be able to track them at **Home** screen. If you have not
specified planned amount for the category, Profitocracy will just calculate and show you the total amount of expenses 
for the category while current month.

# Installation

For now, the only platform you can install without needing to build the application by yourself is to install it to Android device
through *.apk* file.

I want to publish Profitocracy to a Google Play Store and Apple App Store, but it will happen later.

## Android

To install Profitocracy to an Android device, go to [latest release](https://github.com/KrawMire/profitocracy-maui/releases/latest) and install an attached *.apk* file. 
Then click on it and follow the instructions.

> If you want to try out a specific version of Profitocracy, go to 
> [the list of releases](https://github.com/KrawMire/profitocracy-maui/releases) 
> and select the version you would like to install.

# Gettings Started

All the steps were recorded on iOS device, but it is also correct for Android and other operating systems.
There we will look at all the steps to set up **Profitocracy** for comfortable use.

## 1. First Launch

After the first launch of the **Profitocracy** you will be moved to **Setup** screen 
to create your first profile and specify initial balance.

<img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/first_launch.gif" width="250" />

## 2. Appearance

At **Settings** screen you can change application theme (*Light/Dark/System*) and select
a language (*English/Russian*).

<img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/theme_lang.gif" width="250" />

## 3. Creation and Setting Up Categories

First you need to do after your first profile creation is to 
create needed spending categories and specify their planned amounts.

<img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/categories.gif" width="250" />

## 4. Transaction Creation

The application is fully set up and now you are able to create transaction. Let's do it!

<img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/transactions.gif" width="250" />

## 5. Viewing Transactions by Spending Type and Category

On **Home** screen you can tap at spending type name or a category 
name to look at the list of transactions.

<img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/filtered_transactions.gif" width="250" />

