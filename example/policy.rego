package example

default allow = false

allow {
    input.method == "GET"
    input.path == ["weatherforecast"]
    input.user == "anonymous"
}