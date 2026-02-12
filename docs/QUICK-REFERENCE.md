# Quick Reference Guide

Quick commands for building, deploying, and managing CarShieldAPI.

## Build Commands

### Standard Build
```bash
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

### Build for Different Architectures
```bash
# Linux ARM64 (for ARM-based servers)
dotnet publish -c Release -r linux-arm64 --self-contained -o ./publish

# Linux x64 (standard)
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

## Deployment Commands

### Copy to Server
```bash
# Standard SCP
scp -r ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/

# With SSH key
scp -r -i /path/to/key.pem ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/
```

### Set Permissions
```bash
chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI
chmod -R 750 /home/kgaddy/CarShieldAPI/Data
```

## Service Management

### Service Control
```bash
# Start
sudo systemctl start carshieldapi

# Stop
sudo systemctl stop carshieldapi

# Restart
sudo systemctl restart carshieldapi

# Status
sudo systemctl status carshieldapi

# Enable on boot
sudo systemctl enable carshieldapi

# Disable on boot
sudo systemctl disable carshieldapi
```

### Reload Service Configuration
```bash
sudo systemctl daemon-reload
```

## Logging

### View Logs
```bash
# Last 100 lines
sudo journalctl -u carshieldapi -n 100

# Follow live logs
sudo journalctl -u carshieldapi -f

# Since specific time
sudo journalctl -u carshieldapi --since "10 minutes ago"
sudo journalctl -u carshieldapi --since "2026-02-06 20:00:00"

# All logs (no pager)
sudo journalctl -u carshieldapi --no-pager
```

## Testing

### Test API Endpoints
```bash

# Get all projects
curl http://localhost:5000/api/Project/GetProjects

# Get specific project
curl http://localhost:5000/api/Project/GetProject/{id}

# Login (POST)
curl -X POST http://localhost:5000/api/Project/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@test.com","password":"john123"}'
```

### Test from Remote
```bash
# Replace with your server IP
curl http://your-server-ip:5000/WeatherForecast
```

## Troubleshooting

### Check Process
```bash
# Find process
ps aux | grep CarShieldAPI

# Check port usage
sudo netstat -tlnp | grep 5000
sudo ss -tlnp | grep 5000
```

### Check Files
```bash
# List all files
ls -la /home/kgaddy/CarShieldAPI/

# Check for .so files
ls -la /home/kgaddy/CarShieldAPI/ | grep "\.so"

# Check Data folder
ls -la /home/kgaddy/CarShieldAPI/Data/
```

### Run Directly (for debugging)
```bash
cd /home/kgaddy/CarShieldAPI
./CarShieldAPI
```

### Check Permissions
```bash
# Check executable
ls -l /home/kgaddy/CarShieldAPI/CarShieldAPI

# Fix if needed
chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI
```

## File Locations

### Application Files
- **Executable**: `/home/kgaddy/CarShieldAPI/CarShieldAPI`
- **Data**: `/home/kgaddy/CarShieldAPI/Data/`
- **Config**: `/home/kgaddy/CarShieldAPI/appsettings.json`

### Service Files
- **Service File**: `/etc/systemd/system/carshieldapi.service`
- **Logs**: `journalctl -u carshieldapi`

## Common Update Workflow

```bash
# 1. Build on Mac/dev machine
cd /Users/kevingaddy/Documents/NET/CarShieldServices/CarShieldAPI
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish

# 2. Stop service on server
ssh kgaddy@your-server-ip
sudo systemctl stop carshieldapi
exit

# 3. Deploy from Mac/dev machine
scp -r ./publish/* kgaddy@your-server-ip:/home/kgaddy/CarShieldAPI/

# 4. Restart service on server
ssh kgaddy@your-server-ip
chmod +x /home/kgaddy/CarShieldAPI/CarShieldAPI
sudo systemctl start carshieldapi
sudo systemctl status carshieldapi
```

## Environment Variables

### Change Port
Edit service file:
```bash
sudo nano /etc/systemd/system/carshieldapi.service
```

Change:
```ini
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001
```

Reload:
```bash
sudo systemctl daemon-reload
sudo systemctl restart carshieldapi
```

### Change Environment
```ini
Environment=ASPNETCORE_ENVIRONMENT=Development
# or
Environment=ASPNETCORE_ENVIRONMENT=Production
```

## Monitoring

### Resource Usage
```bash
# CPU/Memory
top -p $(pgrep -f CarShieldAPI)

# Or
htop -p $(pgrep -f CarShieldAPI)

# Systemd resource view
systemd-cgtop
```

### Health Check Script
```bash
# Create health check
curl -f http://localhost:5000/WeatherForecast || echo "API is down!"
```

## Firewall

### Open Port
```bash
# UFW (Ubuntu)
sudo ufw allow 5000/tcp
sudo ufw status

# firewalld (CentOS/RHEL)
sudo firewall-cmd --permanent --add-port=5000/tcp
sudo firewall-cmd --reload
```

## Backup

### Backup Data
```bash
# From server
tar -czf carshieldapi-data-backup.tar.gz /home/kgaddy/CarShieldAPI/Data/

# Copy to local machine
scp kgaddy@your-server-ip:~/carshieldapi-data-backup.tar.gz ./
```

### Restore Data
```bash
# Upload to server
scp ./carshieldapi-data-backup.tar.gz kgaddy@your-server-ip:~/

# On server
sudo systemctl stop carshieldapi
tar -xzf ~/carshieldapi-data-backup.tar.gz -C /
sudo systemctl start carshieldapi
```
