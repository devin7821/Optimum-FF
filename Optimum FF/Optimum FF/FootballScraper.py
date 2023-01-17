import sqlite3
from urllib.request import urlopen
from bs4 import BeautifulSoup
import pandas as pd
import pyodbc
import numpy
import re

#DB Info
server = '.\SQLEXPRESS'
database = 'tempdb'
username = 'usermain'
password = '123'

#Create DB Connection
cnxn = pyodbc.connect('DRIVER={SQL Server}; SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+password)
cursor = cnxn.cursor


#QB Scrape Info
year = 2022
url = "https://www.pro-football-reference.com/years/{}/passing.htm".format(year)
html = urlopen(url)
soup = BeautifulSoup(html, features="lxml")

#Get Headers
headers = [th.getText() for th in soup.findAll('tr')[0].findAll('th')]
headers = headers[1:]
#print(headers)

#Get Rows
rows = soup.findAll('tr', class_ = lambda table_rows: table_rows != "thead")
player_stats = [[td.getText() for td in rows[i].findAll('td')]
                for i in range(len(rows))]
player_stats = player_stats[1:]

#Create DataFrame
stats = pd.DataFrame(player_stats, columns = headers)
stats.head()

stats = stats.replace(r'', 0, regex=True)
stats.columns = stats.columns.str.replace(r"[/]", "b", regex=True)
stats.columns = stats.columns.str.replace(r"[%]", "p", regex=True)

"""cols = []
count = 1
for column in stats.columns:
    if column == 'Yds':
        cols.append(f'Yds_{count}')
        count += 1
        continue
    cols.append(column)
stats.columns = cols"""

#print(stats.dtypes)

#print(stats)

#stats.to_csv('2022QBs.csv')

for index, row in stats.iterrows():
    if row.Pos == "QB":
        row.Player = re.sub('[^a-zA-Z. \d\s]', '', row.Player)
        insert_into_qbs = ("""INSERT INTO QBs (Player, Team, Games, Att, TDs, Int, YPG) values(?,?,?,?,?,?,?);""", cnxn)
        qb_to_insert = ([str(row.Player), str(row.Tm), int(row.G), int(row.Att), int(row.TD), int(row.Int), float(row.YbG)])
        cursor.execute(insert_into_qbs, qb_to_insert)

cnxn.commit()
cursor.close()