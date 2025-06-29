# What this thing do?
Short answer: scans your network and show results as a table, like this:  

![image](https://github.com/user-attachments/assets/353c7141-00e6-4715-817a-e9ff00999c01)

# How do i use this thing? 

```bash
./Pinger [CIDR]
```

There the `[CIDR]` is something like:
```bash
- 192.168.1.1/24
- 192.168.0.1/16
- 10.10.10.0/24
- 19.62.0.1/6

etc...
```

You may use 3 options after CIDR:
1. `--only-up`: will show only hosts which send `ICMP_REPLY` to pinger (im: replyed to this app)
2. `[table_height]`: positive digit, means how many hosts will be rendered in 1 COLUMN of the table. If hosts actually more then it, will add some more columns (default value is `30`)
3. (only in v2.0) `--no-hostnames`: disable autoresolving IP into hostnames

How it works:

![image](https://github.com/user-attachments/assets/8e311ef8-dcc7-44ab-97c6-64a3656fb329)

To disable this functionality use `--no-hostnames` as option:

![image](https://github.com/user-attachments/assets/d9979762-cb11-4afc-8a44-19777d6bee9c)

# Examples
![image](https://github.com/user-attachments/assets/0a12a2b1-3391-4750-a177-4ebcb50225ed)
![image](https://github.com/user-attachments/assets/dc80149b-62c8-4167-a45e-4e15050684d6)
![image](https://github.com/user-attachments/assets/50800c58-59da-4370-8fb4-b15bfd1ea05f)

**TIP: As you can see, last 2 args may be passed out of order**
