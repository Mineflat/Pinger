# What thit thing do?
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

You may use 2 options after CIDR:
1. `--only_up`: will show only hosts which send `ICMP_REPLY` to pinger (im: replyed to this app)
2. `[table_height]`: positive digit, means how many hosts will be rendered in 1 COLUMN of the table. If hosts actually more then it, will add some more columns (default value is `30`)

# Examples
![image](https://github.com/user-attachments/assets/0a12a2b1-3391-4750-a177-4ebcb50225ed)
![image](https://github.com/user-attachments/assets/34cc4489-8af5-4e86-aeb6-4cd63131a0b5)
![image](https://github.com/user-attachments/assets/50800c58-59da-4370-8fb4-b15bfd1ea05f)

**TIP: As you can see, last 2 args may be passed out of order**
