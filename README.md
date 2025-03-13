<p align="center">
  <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/banners/main_banner.png" alt="Title image" />
</p>

<p align="left">
    <a href="https://apps.apple.com/rs/app/profitocracy/id6503658740">
      <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/distribution/download_app_store.svg" alt="App Store Download" height="150"/>
    </a>
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

# Installation

> [!NOTE]
> For now, Profitocracy is published only in the Apple App Store. I hope it will be also available at Google Play Market soon.

## Android

To install Profitocracy to an Android device, go to the [latest release](https://github.com/KrawMire/profitocracy-maui/releases/latest) and install an attached *.apk* file.
Then click on it and follow the instructions.

> If you want to try out a specific version of Profitocracy, go to
> [the list of releases](https://github.com/KrawMire/profitocracy-maui/releases)
> and select the version you would like to install.

## iOS

You can download the Profitocracy on the Apple App Store by following [this link](https://apps.apple.com/rs/app/profitocracy/id6503658740) or by clicking on the following iamge:

<p align="left">
    <a href="https://apps.apple.com/rs/app/profitocracy/id6503658740">
      <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/distribution/download_app_store.svg" alt="App Store Download" height="150"/>
    </a>
</p>

# More About Profitocracy

## <a name="503020rule"></a> What is a 50-30-20 rule

The 50-30-20 rule is a common way to allocate the spending categories in your personal or household budget.
The rule targets 50% of your after-tax income toward necessities, 30% toward things you don’t need—but make
life a little nicer and the final 20% toward paying down debt and/or adding to your savings.

## Terminologies Used in Profitocracy

### 💼 Profile

A **Profile** represents all your financial activity in one place. It tracks your expenses and calculates amounts for primary, secondary, and savings expenses. Additionally, it provides insights into your daily spending as well as categorized expenses for better organization.

### 🧾 Transaction

A **Transaction** is an operation that moves funds. It can either be an **income** (e.g., your salary) or an **expense** (e.g., groceries, rent). For expense transactions, you’ll need to specify:
- The **type** of expense: primary, secondary, or savings.
- The **amount** spent.

You can also optionally provide additional details such as:
- **Spending category** (default is "None").
- **Description** of the transaction.
- **Date** of the operation.

### 💵 Actual and Planned Amounts of Expenses

In Profitocracy, most of the data displayed on the **Home Screen** revolves around expenses. An **expense** is represented by two key values:
- **Actual Amount:** The real amount of money spent for a specific type or category.
- **Planned Amount:** The amount Profitocracy suggests as your spending limit for each category or type.

This balance between actual and planned amounts helps you stay in control of your financial goals.

### 📊 Category

A **Category** is a grouping tool for your transactions. Each category can be customized with:
- A **name** (e.g., Food, Entertainment).
- An optional **planned amount** for the month (set during creation on the **Settings Screen**).

Once created, you can track your category-wise spending on the **Home Screen.**

- If a planned amount is set, Profitocracy compares your spending against it for better tracking.
- If no planned amount is defined, the app simply displays the total expenses for the category during the ongoing month.

# Gettings Started

All the steps were recorded on iOS device, but it is also correct for Android and other operating systems.
There we will look at all the steps to set up **Profitocracy** for comfortable use.

| Description                                                                                                                                    | Screen Recording                                                                                                                 |
|------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| 1. **Creating Your First Profile**. Learn how to set up your first profile when launching the app for the first time.                          | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/1_first_profile.gif" />        |
| 2. **Adding a Second Profile**. Discover how to create and manage an another profile directly from the settings page.                          | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/2_second_profile.gif" />       |
| 3. **Switching Profiles**. See how to set the second profile as the current active one for personalized budget tracking.                       | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/3_set_current_profile.gif" />  |
| 4. **Editing a Profile**. Learn how to update or modify an existing profile with ease.                                                         | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/4_profile_edit.gif" />         |
| 5. **Creating Spending Categories**. Walk through the process of creating your first two spending categories to organize expenses effectively. | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/5_first_category.gif" />       |
| 6. **Editing a Category**. Watch how to edit an existing category to refine your financial organization.                                       | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/6_edit_category.gif" />        |
| 7. **Creating a Main Spending Transaction**. See how to add a new transaction with the Main spending type.                                     | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/7_main_transaction.gif" />     |
| 8. **Saving in a Different Currency (EUR)**. Learn how to create a saving transaction in EUR for managing multi-currency budgets.              | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/8_saving_transaction.gif" />   |
| 9. **Recording Income Transactions**. Walk through the steps for adding an income transaction to track earnings.                               | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/9_income.gif" />               |
| 10. **Handling Withdrawals from Savings (EUR)**. Watch how to create a transaction for withdrawing funds from savings in EUR.                  | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/10_withdraw.gif" />            |
| 11. **Visualizing Expenses with Charts**. Explore the overview page to see how expenses are visualized through insightful and detailed charts. | <img src="https://raw.githubusercontent.com/KrawMire/profitocracy/dev/docs/assets/getting_started/11_overview.gif" />            |

