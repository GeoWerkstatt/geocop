import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
} from "@azure/msal-react";
import { Button, Dropdown, DropdownButton } from "react-bootstrap";
import styled from "styled-components";

const AccountContainer = styled.div`
  display: flex;
  flex-direction: column;
`;

const LoggedInButtonGroup = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
`;

const AccountNameContainer = styled.span`
  font-size: 14px;
`;

const LoginButton = styled(Button)`
  width: 100px;
  margin-left: 10px;
`;

export const Login = ({ clientSettings }) => {
  const { instance } = useMsal();
  const activeAccount = instance.getActiveAccount();

  async function login() {
    try {
      const result = await instance.loginPopup({
        scopes: clientSettings?.authScopes,
      });
      instance.setActiveAccount(result.account);
      document.cookie = `geocop.auth=${result.idToken};Path=/;Secure`;
    } catch (error) {
      console.warn(error);
    }
  }

  async function logout() {
    try {
      await instance.logoutPopup();
      document.cookie = "geocop.auth=;expires=Thu, 01 Jan 1970 00:00:00 GMT;Path=/;Secure";
    } catch (error) {
      console.warn(error);
    }
  }

  return (
    <AccountContainer>
      <UnauthenticatedTemplate>
        <LoginButton onClick={login}>Log in</LoginButton>
      </UnauthenticatedTemplate>
      <AuthenticatedTemplate>
        <LoggedInButtonGroup>
          <DropdownButton title="Administration">
            <Dropdown.Item href="/">Datenabgabe</Dropdown.Item>
            <Dropdown.Item href="/admin">Abgabeübersicht</Dropdown.Item>
            <Dropdown.Item href="https://browser.geocop.ch">
              STAC Browser
            </Dropdown.Item>
          </DropdownButton>
          <LoginButton onClick={logout}>Log out</LoginButton>
        </LoggedInButtonGroup>
        <AccountNameContainer>
          Angemeldet als {activeAccount?.username}
        </AccountNameContainer>
      </AuthenticatedTemplate>
    </AccountContainer>
  );
};
