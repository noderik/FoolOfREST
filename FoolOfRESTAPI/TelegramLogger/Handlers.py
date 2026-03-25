import telegram
import Database
from psycopg import Connection
from psycopg.errors import InFailedSqlTransaction
from telegram import Update
from telegram.ext import ContextTypes


async def message_sent(update: Update, context: ContextTypes.DEFAULT_TYPE) -> None:
    message = update.message
    if message == None:
        return
    user = message.from_user    
    if user == None:
        return
    print(f"\033[1;33m{user.name}\033[0m: {message.text} ({message.date.now().strftime('%Y-%m-%d %H:%M:%S%z')})")
    conn: Connection = context.bot_data["database"]
    try:
        Database.writeMessage(conn, message)
    except InFailedSqlTransaction as error:
        conn.rollback()
        print(error)

def error_handler(update: object, context: ContextTypes.DEFAULT_TYPE) -> None:
    if (type(context.error) == telegram.error.Conflict):
        print("\033[31mOther TelegramBot is working right now. Exiting.\033[39m")
        exit()
