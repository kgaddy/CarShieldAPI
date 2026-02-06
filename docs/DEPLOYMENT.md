# CarShieldAPI Deployment Guide

This guide covers deploying the CarShieldAPI as a self-contained service on Linux servers.

## Prerequisites

- Linux server (Ubuntu 20.04 or newer recommended)
- SSH access to the server
- User account with sudo privileges

## Building for Deployment

### 1. Build the Application

From your development machine (Mac/Windows), navigate to the project directory and run:

```bash
cd /Users/kevingaddy/Documents/NET/CarShieldServices/CarShieldAPI

dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

**Build Parameters:**
- `-c Release` - Release configuration (optimized)
- `-r linux-x64` - Target Linux 64-bit systems
- `--self-contained` - Includes .NET runtime (no need to install .NET on server)
- `-o ./publish` - Output directory

**Important:** The `--self-contained` flag is required to include all runtime libraries (`.so` files) needed to run on Linux without installing .NET.

### 2. Verify Build Output

Check that the build includes the executable and runtime libraries:

```bash
ls -la ./publish/
```

You should see:
- `CarShieldAPI` (76KB executable)
- Multiple `.dll` files
- Multiple `.so` files (libcoreclr.so, libhostfxr.so, etc.)
- `Data/` folder with JSON files
- Configuration files (`appsettings.json`, etc.)

## Deploying to Linux Server

### 1. Copy Files to Server

```bash
scp -r ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/
```

Replace:
- `kgaddy` with your server username
- `your-server-ip` with your server's IP address or hostname

If using SSH key authentication:

```bash
scp -r -i /path/to/your/key.pem ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/
```

### 2. Set File Permissions

SSH into your server and set the correct permissions:

```bash
ssh kgaddy@your-server-ip

# Make the executable file executable
chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI

# Set permissions for Data folder (read/write)
chmod -R 750 /home/kgaddy/CarShieldAPI/Data
```

### 3. Test the Application

Before setting up as a service, test that it runs:

```bash
cd /home/kgaddy/CarShieldAPI
./CarShieldAPI
```

Press `Ctrl+C` to stop. If you see errors, check the Troubleshooting section below.

## Setting Up as a System Service

### 1. Create systemd Service File

```bash
sudo nano /etc/systemd/system/carshieldapi.service
```

Paste the following configuration:

```ini
[Unit]
Description=CarShield API Service
After=network.target

[Service]
WorkingDirectory=/home/kgaddy/CarShieldAPI
ExecStart=/home/kgaddy/CarShieldAPI/CarShieldAPI
Restart=always
RestartSec=5
User=kgaddy
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
```

**Configuration Notes:**
- `WorkingDirectory` - Must point to the directory containing the application
- `ExecStart` - Full path to the executable
- `User` - Linux user that will run the service
- `ASPNETCORE_URLS` - Port the API will listen on (change 5000 if needed)
- `Restart=always` - Auto-restart on crashes
- `RestartSec=5` - Wait 5 seconds before restarting

Save and exit (`Ctrl+X`, then `Y`, then `Enter`).

### 2. Enable and Start the Service

```bash
# Reload systemd to recognize the new service
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable carshieldapi

# Start the service
sudo systemctl start carshieldapi

# Check status
sudo systemctl status carshieldapi
```

### 3. Verify Service is Running

```bash
# Check service status
sudo systemctl status carshieldapi

# View live logs
sudo journalctl -u carshieldapi -f

# Test API endpoint
curl http://localhost:5000/WeatherForecast
```

## Service Management Commands

### Check Service Status

```bash
sudo systemctl status carshieldapi
```

### Start/Stop/Restart Service

```bash
# Start
sudo systemctl start carshieldapi

# Stop
sudo systemctl stop carshieldapi

# Restart
sudo systemctl restart carshieldapi
```

### View Logs

```bash
# View recent logs
sudo journalctl -u carshieldapi -n 100

# Follow logs in real-time
sudo journalctl -u carshieldapi -f

# View logs since specific time
sudo journalctl -u carshieldapi --since "10 minutes ago"
```

### Enable/Disable Auto-start

```bash
# Enable (start on boot)
sudo systemctl enable carshieldapi

# Disable (don't start on boot)
sudo systemctl disable carshieldapi
```

## Updating the Application

When deploying updates:

1. **Build new version** on your development machine:
   ```bash
   dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
   ```

2. **Stop the service** on the server:
   ```bash
   sudo systemctl stop carshieldapi
   ```

3. **Copy new files** to server:
   ```bash
   scp -r ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/
   ```

4. **Set permissions** (if needed):
   ```bash
   chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI
   ```

5. **Restart the service**:
   ```bash
   sudo systemctl start carshieldapi
   sudo systemctl status carshieldapi
   ```

## Troubleshooting

### Service Won't Start

1. **Check service status**:
   ```bash
   sudo systemctl status carshieldapi -l
   ```

2. **Check detailed logs**:
   ```bash
   sudo journalctl -u carshieldapi -n 100 --no-pager
   ```

3. **Try running directly**:
   ```bash
   cd /home/kgaddy/CarShieldAPI
   ./CarShieldAPI
   ```
   This will show any error messages.

### Permission Errors

```bash
# Fix executable permissions
chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI

# Fix data folder permissions
chmod -R 750 /home/kgaddy/CarShieldAPI/Data

# Ensure correct ownership
sudo chown -R kgaddy:kgaddy /home/kgaddy/CarShieldAPI
```

### Port Already in Use

If port 5000 is already in use, change it in the service file:

```bash
sudo nano /etc/systemd/system/carshieldapi.service
```

Change:
```ini
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001
```

Then reload and restart:
```bash
sudo systemctl daemon-reload
sudo systemctl restart carshieldapi
```

### Missing Runtime Libraries

If you get errors about missing `.so` files, rebuild with the `--self-contained` flag:

```bash
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

Verify `.so` files are present:
```bash
ls -la ./publish/ | grep "\.so"
```

### Application Crashes (Segmentation Fault)

This usually means the build is incompatible with your server. Ensure:
1. You're using `--self-contained` flag
2. Runtime architecture matches (linux-x64)
3. All `.so` files were copied to the server

### Data Files Not Persisting

Ensure the Data folder has write permissions:

```bash
chmod -R 750 /home/kgaddy/CarShieldAPI/Data
ls -la /home/kgaddy/CarShieldAPI/Data/
```

## Security Considerations

### Firewall Configuration

If using a firewall, allow the API port:

```bash
# For UFW (Ubuntu)
sudo ufw allow 5000/tcp

# For firewalld (CentOS/RHEL)
sudo firewall-cmd --permanent --add-port=5000/tcp
sudo firewall-cmd --reload
```

### HTTPS Configuration

For production, use a reverse proxy (nginx or Apache) with SSL certificates:

1. Install nginx
2. Configure reverse proxy to CarShieldAPI
3. Set up Let's Encrypt SSL certificates
4. Update API to listen on localhost only

Example nginx configuration:

```nginx
server {
    listen 80;
    server_name api.yourdomain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### File Permissions

Keep restrictive permissions:
- Executable: `755` or `750`
- Data files: `750` (read/write for owner, read for group)
- Configuration files: `640` (read/write for owner, read for group)

## Monitoring

### Check Resource Usage

```bash
# CPU and Memory usage
top -p $(pgrep -f CarShieldAPI)

# Or with systemd-cgtop
systemd-cgtop
```

### Automated Health Checks

Set up a cron job to monitor the API:

```bash
crontab -e
```

Add:
```bash
*/5 * * * * curl -f http://localhost:5000/WeatherForecast || systemctl restart carshieldapi
```

## Additional Notes

- **Target Framework**: .NET 10.0
- **Build Type**: Self-contained (includes runtime)
- **Default Port**: 5000
- **Configuration**: `appsettings.json` for settings
- **Data Storage**: JSON files in `Data/` folder
- **Logs**: Available via `journalctl`

## Support

For issues or questions:
1. Check logs: `sudo journalctl -u carshieldapi -n 100`
2. Verify file permissions
3. Ensure all files were copied during deployment
4. Check that port is not in use by another service
