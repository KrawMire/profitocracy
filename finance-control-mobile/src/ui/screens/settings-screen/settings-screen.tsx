import { Keyboard, ScrollView, TouchableWithoutFeedback, View } from "react-native";
import { useDispatch } from "react-redux";

import { AppThemeSettings } from "../../components/settings-screen/app-theme-settings";
import { resetStore } from "state/global/actions";
import { settingsScreenStyles } from "styles/screens/settings.style";
import { sharedTextStyle } from "styles/shared/text.style";
import { Divider } from "components/shared/divider";
import { TotalBalanceSettings } from "components/settings-screen/total-balance-settings";
import { BillingPeriodsSettings } from "components/settings-screen/billing-periods-settings";
import { ExpensesSettings } from "components/settings-screen/expenses-settings";
import { ExpensesCategoriesSettings } from "components/settings-screen/expenses-categories-settings";
import { Button, Layout, Text } from "@ui-kitten/components";

export function SettingsScreen() {
  const dispatch = useDispatch();

  const onResetApp = () => {
    dispatch(resetStore());
  }

  return (
    <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
      <ScrollView>
        <Layout style={settingsScreenStyles.wrapper}>
          <Text category="h1">Settings</Text>
          <TotalBalanceSettings />
          <Divider />
          <BillingPeriodsSettings />
          <Divider />
          <AppThemeSettings />
          <Divider />
          <ExpensesSettings />
          <Divider />
          <ExpensesCategoriesSettings />
          <Divider />
          <Button
            onPress={onResetApp}
            status="danger"
            style={{
              // TODO: Temporary solution
              marginTop: 25
            }}
          >
            Reset app
          </Button>
        </Layout>
      </ScrollView>
    </TouchableWithoutFeedback>
  )
}