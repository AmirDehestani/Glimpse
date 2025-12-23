# Glimpse

## Dependencies

- .NET SDK 8.x
- systemd (Linux)

## Installation

1. Clone the repository:

```bash
git clone https://github.com/AmirDehestani/Glimpse.git
cd glimpse
```

2. Run the install script:

```bash
sudo ./scripts/install.sh
```

This will:

- Publish the Worker for `linux-x64` (Release) in `/usr/lib/glimpse`
- Install the systemd unit to `/usr/lib/systemd/system/glimpse.service`
- Reload systemd

3. Enable and start the service:

```bash
sudo systemctl enable --now glimpse
```

Check logs:

```bash
journalctl -u glimpse -f
```
