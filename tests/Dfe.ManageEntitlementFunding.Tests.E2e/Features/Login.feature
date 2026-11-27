Feature: Login

Background:
    Given I am on the provider portal homepage

Scenario: Homepage title
    Then the page title is "Manage entitlement funding for early years providers - GOV.UK"

Scenario: Log in page contents
    When I click the "Start now" button
    Then the page title is "Development Sign-In - GOV.UK"
    And I can see the log in fields
    And I can see the "Sign in with test identity" button

Scenario: Developer log in
    When I click the "Start now" button
    When I enter the developer log in details
    And I click the "Sign in with test identity" button
    Then I am signed in as the developer user

Scenario: Developer log out
    When I log in
    And I click the "Dev Sign out" button
    Then I see the homepage
