#!/bin/sh
cd "$(dirname "$0")"
base64 -d ModlinAirport.rbxmx.gz.b64 | gunzip > ModlinAirport.rbxmx && echo "Gotowe! Powstal plik ModlinAirport.rbxmx - zaimportuj go w Roblox Studio."
