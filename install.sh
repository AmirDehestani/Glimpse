#!/bin/bash
set -e

SERVICE=glimpse.service
PREFIX=/usr

echo "Publishing Worker..."
dotnet publish src/Worker -p:PublishProfile=linux-x64

echo "Installing systemd unit..."
install -Dm644 \
  $SERVICE \
  $PREFIX/lib/systemd/system/$SERVICE
systemctl daemon-reload

echo "Enable with: systemctl enable --now glimpse.service"
