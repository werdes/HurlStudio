# A very simple Hurl file
# with tasty comments...
GET https://www.sample.net
x-app: MY_APP  # Add a dummy header

HTTP 302       # Check that we have a redirection
[Asserts]
header "Location" exists
header "Location" contains "login"  # Check that we are redirected to the login page