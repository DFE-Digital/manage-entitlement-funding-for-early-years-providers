Feature: Login

Background:
    Given I am on the provider portal homepage

Scenario: Homepage title
    Then the page title is "Manage entitlement funding for early years providers - GOV.UK"

Scenario: Log in page contents
    When I click to start
    Then the page title is "Development Sign-In - GOV.UK"
    And I can see the log in fields
    And I can see the sign in button

Scenario: Developer log in
    When I click to start
    When I enter the developer log in details
    And I click the sign in button
    Then I am signed in as the developer user

Scenario: Developer log out
    When I log in
    And I click the "Dev Sign out" button
    Then I see the homepage
