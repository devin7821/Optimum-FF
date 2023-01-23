import sqlite3
from urllib.request import urlopen
from bs4 import BeautifulSoup
import pandas as pd
import pyodbc
import re
from datetime import datetime
import sys

#DB Info
server = 'offsqlserver.database.windows.net'
database = 'OFF'
username = 'mainuser'
password = 'optimumfootball3!'

#Team Dictionary
teams = {
    "Arizona Cardinals": "ARI",
    "Atlanta Falcons": "ATL",
    "Baltimore Ravens": "BAL",
}

#Get Current Season
month = datetime.now().month
year = datetime.now().year
if (month < 8):
    year = year - 1
    
def create_game_table(curr):
    create_table_command = ("""IF OBJECT_ID(N'dbo.Games', N'U') IS NULL
                                        CREATE TABLE [dbo].[Games] (
                                            [Week] [int] NOT NULL,
                                            [Home] [varchar](50) NOT NULL,
                                            [Away] [varchar](50) NOT NULL)""")
    curr.execute(create_table_command)

def create_team_table(curr):
    create_table_command = ("""IF OBJECT_ID(N'dbo.Teams', N'U') IS NULL
                                        CREATE TABLE [dbo].[Teams] (
                                            [Team] [varchar](50) NOT NULL,
                                            [Games] [int] NULL,
                                            [PTD] [int] NULL,
                                            [RTD] [int] NULL,
                                            [TO] [int] NULL,
                                            [Int] [int] NULL,
                                            [RETYPG] [int] NULL,
                                            [DTD] [int] NULL)""")
    curr.execute(create_table_command)

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

def AddGames(cursor, cnxn):      
    def check_if_game_exists(curr, home, away):
        query = ("""SELECT Home, Away FROM Games WHERE Home = ? AND Away = ?""")
        vars = ([home, away])
        curr.execute(query, vars)
        
        return curr.fetchone() is not None

    #QB Scrape Info    
    url = "https://www.pro-football-reference.com/years/{}/games.htm#games".format(year)
    html = urlopen(url)
    soup = BeautifulSoup(html, features="lxml")

    #Get Headers
    headers = [th.getText() for th in soup.findAll('tr')[0].findAll('th')]
    headers = headers[0:]

    #Get Rows
    rows = soup.findAll('tr', class_ = lambda table_rows: table_rows != "thead")
    player_stats = []
    j = 0
    for row in rows:
        player_stats.append([th.getText() for th in rows[j].findAll('th', {'data-stat': 'week_num'})])
        player_stats[j].extend([td.getText() for td in rows[j].findAll('td')])
        j += 1
    #player_stats = [[td.getText() for td in rows[i].findAll('td', 'th')] for i in range(len(rows))]
    player_stats = player_stats[1:]

    #Create DataFrame
    stats = pd.DataFrame(player_stats, columns = headers)
    stats.head()

    stats = stats.replace(r'', None, regex=True)
    stats.columns.values[5] = 'Location'
    stats.columns = stats.columns.str.replace(r"[/]", "b", regex=True)
    stats.columns = stats.columns.str.replace(r"[%]", "p", regex=True)

    cnxn.commit()

    for index, row in stats.iterrows():
        if str(row.Week).isnumeric():
            insert_into_games = ("""INSERT INTO Games (Week, Home, Away) values(?,?,?);""")
            if row.Location is None:
                if not check_if_game_exists(cursor, str(row.Winnerbtie), str(row.Loserbtie)):
                    game_to_insert = ([int(row.Week), str(row.Winnerbtie), str(row.Loserbtie)])
                    cursor.execute(insert_into_games, game_to_insert)
            else:
                if not check_if_game_exists(cursor, str(row.Loserbtie), str(row.Winnerbtie)):
                    game_to_insert = ([int(row.Week), str(row.Loserbtie), str(row.Winnerbtie)])
                    cursor.execute(insert_into_games, game_to_insert)

    cnxn.commit()

def AddTeamsPassing(cursor, cnxn):      
    def update_team_row(curr, row):
        query = ("""UPDATE Teams 
                    SET Games = ?,
                        PTD = ?,
                        Int = ?,
                        Sk = ?,
                    WHERE Team = ?;""")
        vars = ([int(row.G), int(row.TD), int(row.Int), int(row.Sk), str(row.Tm)])
        curr.execute(query, vars)

    #QB Scrape Info    
    url = "https://www.pro-football-reference.com/years/{}/opp.htm#passing".format(year)
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

    for index, row in stats.iterrows():
        if row.Pos == "Team":
            row.Player = re.sub('[^a-zA-Z. \d\s]', '', row.Player)
            if check_if_exists(cursor, row.Tm, "Teams"):
                update_team_row(cursor, row)
            else:
                insert_into_teams = ("""INSERT INTO Teams (Team, Games, PTD, Int, Sk) values(?,?,?,?,?);""")
                team_to_insert = ([str(row.Tm), int(row.G), int(row.TD), int(row.Int), int(row.Sk)])
                cursor.execute(insert_into_teams, team_to_insert)

    cnxn.commit()

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
    connectionString = str(sys.argv[1])
    #Create DB Connection
    #cnxn = pyodbc.connect('DRIVER={SQL Server}; SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+password)
    cnxn = pyodbc.connect('DRIVER={SQL Server};' + connectionString)
    cursor = cnxn.cursor()
    
    #Add Tables
    create_game_table(cursor)
    create_team_table(cursor)
    create_qb_table(cursor)
    create_wr_table(cursor)
    create_rb_table(cursor)
    create_te_table(cursor)
    
    #Add Players to DB
    AddGames(cursor, cnxn)

    AddQBs(cursor, cnxn)
    AddReceiving(cursor, cnxn)
    
    #Close cursor
    cursor.close()
    cnxn.close()
    
if __name__ == "__main__":
    main()