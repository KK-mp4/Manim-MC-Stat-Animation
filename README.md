# [Manim](https://3b1b.github.io/manim/)-MC-Stat-Animation (work in progress)
## Automatic SQL database (with Minecraft stat files) visualisation using Manim

Example output for **useItem.minecraft.diamond_pickaxe**:<br/>

https://user-images.githubusercontent.com/103208695/178838964-d70ea48f-d918-4d54-a552-2c1db81a3361.mp4

2 years ago I got an idea for storing Minecraft stat files each autosave of Dugged SMP, meaning that after some time we will have a database with all the stats growing over time. This will be handy in displaying charts during timelapses or in analyzing data. So I asked [Robi](https://github.com/Robitobi01) to start saving stats automatically.<br/> 
![image](https://user-images.githubusercontent.com/103208695/178839882-44c8c721-642d-4bb5-ba78-73209a431704.png)

He is also way better than me in SQL and managed to save space, instead of storing all 1879 stats of all players for each date (3.8 GiB after 2+ years), we only store stat file if it got changed from last time (now only 20.5 Mib).
So here is what I did:

### 1. Preparing SQL
I downloaded db file from phpMyAdmin and imported it into Microsoft SQL Server with correct mappings.<br/> 

![image](https://user-images.githubusercontent.com/103208695/178842235-f5b195f0-9868-4982-b9f6-0bcc634c9596.png)

### 2. Generating manim file
Wrote C# winforms program to connect to SQL and generate manim file with some parameters and date selection.<br/> 

![image](https://user-images.githubusercontent.com/103208695/178841980-28bfa04b-f62f-4d81-9045-f18b9a6b3880.png)

Here is a dictionary what stat_id reffers to (Excel file in the project):<br/> 

![image](https://user-images.githubusercontent.com/103208695/178843649-ee91ed71-cc87-47cf-b3cf-1ca1d9dcae74.png)


Here is what an output python file will look like:<br/> 

![image](https://user-images.githubusercontent.com/103208695/178842933-d7029db2-36c5-411e-9d7d-6cfaa4763125.png)

### 3. Installing Manim
To actually generate a video you will need to install a python library called Manim Community version - it's an animation engine for explanatory math videos mainly developed by [3Blue1Brown](https://www.youtube.com/channel/UCYO_jab_esuFRV4b17AJtAw)<br/> 
  [How to install it](https://docs.manim.community/en/stable/installation/windows.html)<br/> 
  [How to configure Visual Studio Code to work with Manim](https://naveen521kk.github.io/manim/manim-configuration-for-vscode/)<br/> 

### 4. Running Manim
After installation and preperation of IDE of your chose you will have to run the script ether by running this powershell command<br/> 
**manim -pql main.py Chart -r 1920,1080 --fps 60**<br/> 
or by using Manim Sideview extention for VSCode

To learn more about Manim I suggest reading its comunity version [documentation](https://docs.manim.community/en/stable/)
