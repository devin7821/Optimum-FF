import sqlite3
from urllib.request import urlopen
from bs4 import BeautifulSoup
import pandas as pd
import pyodbc
import numpy
import re
from datetime import datetime

#DB Info
server = 'offsqlserver.database.windows.net'
database = 'OFF'
username = 'mainuser'
password = 'optimumfootball3!'

#Get Current Season
month = datetime.now().month
year = datetime.now().year
if (month < 8):
    year = year - 1

def create_qb_table(curr):
        create_table_command = ("""IF OBJECT_ID(N'dbo.QBs', N'U') IS NULL
                                    CREATE TABLE [dbo].[QBs] (
                                        [Player] [varchar](50) NOT NULL,
                                        [Team] [varchar](50) NOT NULL,
                                        [Games] [int] NOT NULL,
                                        [Att] [int] NOT NULL,
                                        [TDs] [int] NOT NULL,
                                        [Int] [int] NOT NULL,
                                        [YPG] [float] NOT NULL)""")
        curr.execute(create_table_command)
        
def create_wr_table(curr):
        create_table_command = ("""IF OBJECT_ID(N'dbo.WRs', N'U') IS NULL
                                    CREATE TABLE [dbo].[WRs] (
                                        [Player] [varchar](50) NULL,
                                        [Team] [varchar](50) NULL,
                                        [Games] [int] NULL,
                                        [Att] [int] NULL,
                                        [RCPG] [float] NULL,
                                        [RCTDs] [int] NULL,
                                        [RUTDs] [int] NULL,
                                        [RCFmb] [int] NULL,
                                        [RUFmb] [int] NULL,
                                        [RCYPG] [float] NULL,
                                        [RUYPG] [float] NULL)""")
        curr.execute(create_table_command)

def create_rb_table(curr):
        create_table_command = ("""IF OBJECT_ID(N'dbo.RBs', N'U') IS NULL
                                    CREATE TABLE [dbo].[RBs] (
                                        [Player] [varchar](50) NULL,
                                        [Team] [varchar](50) NULL,
                                        [Games] [int] NULL,
                                        [Att] [int] NULL,
                                        [RCPG] [float] NULL,
                                        [RCTDs] [int] NULL,
                                        [RUTDs] [int] NULL,
                                        [RCFmb] [int] NULL,
                                        [RUFmb] [int] NULL,
                                        [RCYPG] [float] NULL,
                                        [RUYPG] [float] NULL)""")
        curr.execute(create_table_command)
        
def create_te_table(curr):
        create_table_command = ("""IF OBJECT_ID(N'dbo.TEs', N'U') IS NULL
                                    CREATE TABLE [dbo].[TEs] (
                                        [Player] [varchar](50) NULL,
                                        [Team] [varchar](50) NULL,
                                        [Games] [int] NULL,
                                        [Att] [int] NULL,
                                        [RCPG] [float] NULL,
                                        [RCTDs] [int] NULL,
                                        [RUTDs] [int] NULL,
                                        [RCFmb] [int] NULL,
                                        [RUFmb] [int] NULL,
                                        [RCYPG] [float] NULL,
                                        [RUYPG] [float] NULL)""")
        curr.execute(create_table_command)

def check_if_exists(curr, player, table):
    query = ("""SELECT Player FROM """ + table + """ WHERE Player = ?""")
    vars = (player,)
    curr.execute(query, vars)
    
    return curr.fetchone() is not None

def AddQBs(cursor, cnxn):      
    def update_qb_row(curr, row):
        query = ("""UPDATE QBs 
                    SET Team = ?,
                        Games = ?,
                        Att = ?,
                        TDs = ?,
                        Int = ?,
                        YPG = ?
                    WHERE Player = ?;""")
        vars = ([str(row.Tm), int(row.G), int(row.Att), int(row.TD), int(row.Int), float(row.YbG), str(row.Player)])
        curr.execute(query, vars)

    #QB Scrape Info    
    url = "https://www.pro-football-reference.com/years/{}/passing.htm".format(year)
    html = urlopen(url)
    soup = BeautifulSoup(html, features="lxml")

    #Get Headers
    headers = [th.getText() for th in soup.findAll('tr')[0].findAll('th')]
    headers = headers[1:]

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
    
    create_qb_table(cursor)
    cnxn.commit()

    for index, row in stats.iterrows():
        if row.Pos == "QB":
            row.Player = re.sub('[^a-zA-Z. \d\s]', '', row.Player)
            if check_if_exists(cursor, row.Player, "QBs"):
                update_qb_row(cursor, row)
            else:
                insert_into_qbs = ("""INSERT INTO QBs (Player, Team, Games, Att, TDs, Int, YPG) values(?,?,?,?,?,?,?);""")
                qb_to_insert = ([str(row.Player), str(row.Tm), int(row.G), int(row.Att), int(row.TD), int(row.Int), float(row.YbG)])
                cursor.execute(insert_into_qbs, qb_to_insert)

    cnxn.commit()
    
def AddReceiving(cursor, cnxn):    
    def update_wr_row(curr, row):
        query = ("""UPDATE WRs 
                    SET Team = ?,
                        Games = ?,
                        RCPG = ?,
                        RCTDs = ?,
                        RCFmb = ?,
                        RCYPG = ?
                    WHERE Player = ?;""")
        vars = ([str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG), str(row.Player)])
        curr.execute(query, vars)
        
    def update_rb_row(curr, row):
        query = ("""UPDATE RBs 
                    SET Team = ?,
                        Games = ?,
                        RCPG = ?,
                        RCTDs = ?,
                        RCFmb = ?,
                        RCYPG = ?
                    WHERE Player = ?;""")
        vars = ([str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG), str(row.Player)])
        curr.execute(query, vars)
        
    def update_te_row(curr, row):
        query = ("""UPDATE TEs 
                    SET Team = ?,
                        Games = ?,
                        RCPG = ?,
                        RCTDs = ?,
                        RCFmb = ?,
                        RCYPG = ?
                    WHERE Player = ?;""")
        vars = ([str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG), str(row.Player)])
        curr.execute(query, vars)
        
    #WR Scrape Info
    url = "https://www.pro-football-reference.com/years/{}/receiving.htm".format(year)
    html = urlopen(url)
    soup = BeautifulSoup(html, features="lxml")

    #Get Headers
    headers = [th.getText() for th in soup.findAll('tr')[0].findAll('th')]
    headers = headers[1:]

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
    
    create_wr_table(cursor)
    cnxn.commit()

    for index, row in stats.iterrows():
        
        row.Player = re.sub('[^a-zA-Z. \d\s]', '', row.Player)
        if row.Pos == "WR":
            if check_if_exists(cursor, row.Player, "WRs"):
                update_wr_row(cursor, row)
            else:
                insert_into_wrs = ("""INSERT INTO WRs (Player, Team, Games, RCPG, RCTDs, RCFmb, RCYPG) values(?,?,?,?,?,?,?);""")
                wr_to_insert = ([str(row.Player), str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG)])
                cursor.execute(insert_into_wrs, wr_to_insert)
                
        if row.Pos == "RB":
            if check_if_exists(cursor, row.Player, "RBs"):
                update_rb_row(cursor, row)
            else:
                insert_into_rbs = ("""INSERT INTO RBs (Player, Team, Games, RCPG, RCTDs, RCFmb, RCYPG) values(?,?,?,?,?,?,?);""")
                rb_to_insert = ([str(row.Player), str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG)])
                cursor.execute(insert_into_rbs, rb_to_insert)
                
        if row.Pos == "TE":
            if check_if_exists(cursor, row.Player, "TEs"):
                update_te_row(cursor, row)
            else:
                insert_into_tes = ("""INSERT INTO TEs (Player, Team, Games, RCPG, RCTDs, RCFmb, RCYPG) values(?,?,?,?,?,?,?);""")
                te_to_insert = ([str(row.Player), str(row.Tm), int(row.G), float(row.RbG), int(row.TD), int(row.Fmb), float(row.YbG)])
                cursor.execute(insert_into_tes, te_to_insert)

    cnxn.commit()

def main():
    #Create DB Connection
    cnxn = pyodbc.connect('DRIVER={SQL Server}; SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+password)
    cursor = cnxn.cursor()
    
    #Add Tables
    create_qb_table(cursor)
    create_wr_table(cursor)
    create_rb_table(cursor)
    create_te_table(cursor)
    
    #Add Players to DB
    AddQBs(cursor, cnxn)
    AddReceiving(cursor, cnxn)
    
    #Close cursor
    cursor.close()
    
if __name__ == "__main__":
    main()