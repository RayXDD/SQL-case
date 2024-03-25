// See https://aka.ms/new-console-template for more information

/*SELECT*/
/*1.Recyclable and Low Fat Products*/
SELECT product_id FROM Products WHERE low_fats ='Y' AND recyclable ='Y'
/* 2.Find Customer Referee */
SELECT [name] FROM Customer WHERE referee_id != 2 OR referee_id IS NULL
/* 3.Big Countries */
SELECT [name],[population],[area] FROM World WHERE area >= 3000000 OR population >= 25000000
/* 4.Article Views I */
SELECT DISTINCT author_id as id FROM Views WHERE author_id = viewer_id ORDER BY id
/* 5.Invalid Tweets */
SELECT tweet_id FROM Tweets WHERE LEN(content) > 15


/*Basic Joins*/
/*1.Replace Employee ID With The Unique Identifier */
SELECT B.unique_id,A.name FROM Employees A
LEFT JOIN EmployeeUNI B ON A.id = B.id
/* 2.Product Sales Analysis I */
SELECT B.product_name,A.year,A.price FROM Sales A
LEFT JOIN Product B ON A.product_id = B.product_id
/* 3.Customer Who Visited but Did Not Make Any Transactions */
SELECT A.customer_id,COUNT(A.visit_id) as count_no_trans FROM Visits A
LEFT JOIN Transactions B ON A.visit_id = B.visit_id
WHERE B.visit_id IS NULL
GROUP BY A.customer_id
/* 4.Rising Temperature */
SELECT a.id FROM Weather A
LEFT JOIN Weather B ON DATEADD(day,-1,A.recordDate) =B.recordDate
WHERE A.temperature > B.temperature
/* 5.Average Time of Process per Machine:注意:Step1:將初始時間、結束時間至同一列 */
SELECT A.machine_id,round(AVG(B.timestamp-A.timestamp),3) as processing_time FROM Activity A
INNER JOIN Activity B 
ON A.machine_id = B.machine_id AND A.process_id = B.process_id AND A.timestamp < B.timestamp
group by A.machine_id
/* 6.Employee Bonus */
SELECT A.name,B.bonus FROM Employee A
FULL JOIN Bonus B ON A.empId = B.empId
WHERE B.bonus < 1000 OR B.bonus IS NULL
/* 7.Students and Examinations */
SELECT A.student_id,A.student_name,B.subject_name,COUNT(C.student_id) as attended_exams FROM Students A
cross join Subjects B
FULL JOIN Examinations C ON A.student_id = C.student_id AND B.subject_name = C.subject_name
GROUP BY A.student_id,A.student_name,B.subject_name
/* 8.Managers with at Least 5 Direct Reports */
SELECT A.name FROM Employee A
LEFT JOIN Employee B ON A.id = B.managerId
GROUP BY A.id,A.name
HAVING COUNT(B.id) >= 5
/* 9.Confirmation Rate 使用CASE WHEN*/
SELECT A.user_id,
ROUND(AVG(CASE
        WHEN B.action = 'confirmed' THEN 1.0
        ELSE 0.0
        END),2) AS confirmation_rate
FROM Signups A
LEFT JOIN Confirmations B ON A.user_id = B.user_id
GROUP BY A.user_id



/*Basic Aggregate Functions*/
/* 1.Not Boring Movies*/
SELECT * FROM Cinema
WHERE id%2=1 AND [description] != 'boring'
ORDER BY rating DESC
/* 2.Average Selling Price  使用ISNULL*/
SELECT A.product_id,ISNULL(ROUND(SUM(A.price * B.units)*1.0/SUM(B.units)*1.0,2),0) as average_price FROM Prices A
LEFT JOIN UnitsSold B ON A.product_id = B.product_id AND (B.purchase_date BETWEEN A.start_date AND A.end_date)
GROUP BY A.product_id 
/* 3.Project Employees I */
SELECT A.project_id,ROUND(AVG(B.experience_years*1.0),2) as average_years FROM Project A
LEFT JOIN Employee B ON A.employee_id = B.employee_id
GROUP BY A.project_id
/* 4.Percentage of Users Attended a Contest 使用子查詢 */
SELECT A.contest_id,ROUND((COUNT(A.user_id)*1.0/(SELECT COUNT(user_id) FROM Users) * 1.0)*100,2) as percentage FROM Register A
LEFT JOIN Users B ON A.user_id = B.user_id
GROUP BY A.contest_id 
ORDER BY [percentage] DESC,contest_id ASC
/* 5.Queries Quality and Percentage */
SELECT query_name,
ROUND(AVG(rating*1.0/position*1.0),2) as quality,
ROUND((SUM(CASE WHEN rating < 3 THEN 1 ELSE 0 END)*1.0/COUNT(*)*1.0)*100,2) as poor_query_percentage
FROM Queries
WHERE query_name IS NOT NULL
GROUP BY query_name
ORDER BY quality ASC
/*6.Monthly Transactions I*/
SELECT 
LEFT(Convert(varchar,trans_date,21),7) as month,
country,
COUNT(amount) as trans_count,
SUM(CASE WHEN state = 'approved' THEN 1 ELSE 0 END) as approved_count,
SUM(amount) as trans_total_amount,
SUM(CASE WHEN state = 'approved' THEN amount ELSE 0 END) as approved_total_amount
FROM Transactions
GROUP BY LEFT(Convert(varchar,trans_date,21),7),country
ORDER BY LEFT(Convert(varchar,trans_date,21),7) ASC,country DESC
/*7.Immediate Food Delivery II*/
WITH CTE AS(
    SELECT 
    customer_id,
    order_date,
    customer_pref_delivery_date,
    (CASE WHEN order_date=customer_pref_delivery_date THEN 1 ELSE 0 END ) as schedule,
    ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY order_date ASC) as ROW_ID
    FROM Delivery
)

SELECT 
    ROUND((SUM(schedule)*100)/(COUNT(customer_id)*1.0),2)as immediate_percentage
FROM CTE
WHERE ROW_ID = 1

/* 8.Game Play Analysis IV */
with first_login as
(SELECT player_id,MIN(event_date) as first_login,DATEADD(day,1,MIN(event_date)) as second_login FROM Activity
GROUP BY player_id)

SELECT ROUND((COUNT(*) * 1.0) / (SELECT COUNT(distinct player_id) FROM Activity) * 1.0,2) as fraction FROM Activity A
JOIN first_login B ON A.event_date = B.second_login AND A.player_id = B.player_id

/*Basic Aggregate Functions*/
/* 1.User Activity for the Past 30 Days I */
SELECT activity_date as day,COUNT(DISTINCT user_id) as active_users FROM Activity
WHERE activity_date BETWEEN DATEADD(day,-29,'2019-07-27') AND '2019-07-27'
GROUP BY activity_date
/* 2.Number of Unique Subjects Taught by Each Teacher */
SELECT teacher_id,COUNT(DISTINCT subject_id) as cnt FROM Teacher
GROUP BY teacher_id
/* 3.Product Sales Analysis III */
SELECT product_id, year AS first_year, quantity, price FROM Sales 
WHERE (product_id, year) IN (
    SELECT product_id, MIN(year) AS year FROM Sales 
    GROUP BY product_id);
/* 4.Write your T-SQL query statement below */
SELECT Class FROM Courses
GROUP BY class
HAVING COUNT(class) >= 5
/* 5.Find Followers Count */
SELECT user_id,COUNT(follower_id) as followers_count FROM Followers
GROUP BY user_id
/* 6.Biggest Single Number*/
SELECT MAX(num) as num FROM MyNumbers
WHERE num IN (SELECT * FROM MyNumbers
GROUP BY num
HAVING COUNT(num) = 1)
/* 7.Customers Who Bought All Products--善用DISTINCT*/
SELECT customer_id FROM Customer
GROUP BY customer_id 
HAVING COUNT(DISTINCT product_key) = (SELECT COUNT(product_key) FROM Product) 


/*Advanced Select and Joins*/
/* 1.The Number of Employees Which Report to Each Employee 四捨五入要記得改成浮點數*/
SELECT A.employee_id,A.name,COUNT(B.employee_id) as reports_count,ROUND(AVG(B.age*1.0),0) as average_age FROM Employees A
LEFT JOIN Employees B ON A.employee_id=B.reports_to
GROUP BY  A.employee_id,A.name
HAVING COUNT(B.employee_id) >= 1
ORDER BY employee_id
/* 2.Primary Department for Each Employee */
SELECT employee_id,department_id FROM Employee
WHERE primary_flag = 'Y' OR employee_id IN (SELECT employee_id FROM Employee
GROUP BY employee_id
HAVING COUNT(department_id) = 1)
/* 3.Triangle Judgement */
SELECT x,y,z,(CASE 
WHEN x+y<=z then 'No'
WHEN y+z<=x then 'No'
WHEN x+z<=y then 'No'
ELSE 'Yes' END
) as triangle FROM Triangle 
/*4.Consecutive Numbers */
SELECT DISTINCT A.num as ConsecutiveNums FROM Logs A,Logs B,Logs C
WHERE A.id = B.id + 1 
AND B.id = C.id + 1 
AND A.num = B.num 
AND B.num = C.num
/* 5.Product Price at a Given Date --注意UNION與排序*/
SELECT product_id,
(First_value(new_price) OVER (PARTITION BY product_id ORDER BY change_date DESC )) as price FROM Products
WHERE change_date <= '2019-08-16'
UNION
SELECT product_id,10 FROM Products
GROUP BY product_id
HAVING MIN(change_date) > '2019-08-16'
/* 6.Last Person to Fit in the Bus --利用TOP(1)與SUM(weight)依序相加*/
with Second as
(SELECT person_id,person_name,[weight],turn, 
SUM(weight) OVER (ORDER BY turn ASC)as total FROM [Queue])

SELECT TOP(1) person_name FROM Second
WHERE total <= 1000
ORDER BY turn DESC
/* 7.Count Salary Categories */
SELECT 'Low Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income < 20000
UNION
SELECT 'Average Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income BETWEEN 20000 AND 50000
UNION
SELECT 'High Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income > 50000


/* Subqueries */
/* 1.Employees Whose Manager Left the Company */
SELECT employee_id FROM Employees
WHERE salary < 30000 AND manager_id NOT IN (SELECT employee_id FROM Employees)
ORDER BY employee_id 
/* 2.Exchange Seats --數據平移，LEAD()和LAG() */
SELECT (CASE WHEN ID%2=0 THEN ID-1
             WHEN ID%2=1 THEN LEAD(ID,1,ID) OVER (ORDER BY ID ASC) END) as id,student FROM Seat
ORDER BY id ASC
/* 3.Movie Rating */
WITH results1 AS(
    SELECT TOP(1) C.name as results FROM MovieRating A
LEFT JOIN Users C ON A.user_id = C.user_id
GROUP BY C.name
ORDER BY COUNT(A.rating) DESC,C.name ASC
),results2 AS(
    SELECT TOP(1) B.title as results FROM MovieRating A
LEFT JOIN Movies B ON A.movie_id = B.movie_id
WHERE A.created_at like '2020-02%'
GROUP BY B.title
ORDER BY AVG(A.rating + 0.00) DESC,B.title ASC
)

SELECT results FROM results1
UNION all
SELECT results FROM results2
/* 4.Restaurant Growth --Rows Between來加總特定區間的值*/
SELECT visited_on,
    SUM(SUM(amount)) OVER (ORDER BY visited_on ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) as amount,
    ROUND(CAST(SUM(SUM(amount)) OVER (ORDER BY visited_on ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) as float)/7.0,2) as average_amount
    FROM Customer
GROUP BY visited_on
ORDER BY visited_on
offset 6 ROWS
/* 5.Friend Requests II: Who Has the Most Friends */
WITH Answer AS (
    SELECT requester_id as id FROM RequestAccepted
    UNION ALL
    SELECT accepter_id as id FROM RequestAccepted
)
SELECT TOP(1) id,COUNT(id) as num FROM Answer
GROUP BY id
ORDER BY COUNT(id) DESC
/* 6.Investments in 2016 --窗口函數會計算與它相同的行數。*/
WITH Answer AS(
    SELECT
	*,
	COUNT(lat) OVER(PARTITION BY lat,lon) CountLatLon,
	COUNT(tiv_2015) OVER(PARTITION BY tiv_2015) CountT_2015
	FROM　Insurance 
)
SELECT ROUND(SUM(tiv_2016),2) as tiv_2016 FROM Answer
WHERE CountLatLon = 1 AND CountT_2015 >1

/* 7.Department Top Three Salaries --DENSE_RANK()資料排序w */
WITH Answer as(
SELECT B.name as Department,A.name as Employee,A.salary as Salary,
DENSE_RANK() OVER (PARTITION BY B.name ORDER BY A.salary DESC) as rank FROM Employee A
LEFT JOIN Department B ON A.departmentId = B.id
)

SELECT Department,Employee,Salary FROM Answer
WHERE rank <= 3
ORDER BY Department ASC,Salary DESC

/* Advanced String Functions / Regex / Clause*/
/* 1.Fix Names in a Table --透過UPPER調整字串*/
SELECT user_id,UPPER(LEFT(name,1))+LOWER(RIGHT(name,LEN(name)-1))as name FROM Users
ORDER BY user_id ASC
/* 2.Patients With a Condition*/
SELECT * FROM Patients
WHERE conditions LIKE '% DIAB1%' OR conditions LIKE 'DIAB1%'
/* 3.Delete Duplicate Emails --透過ROW_NUMBER()排序*/
with temp as(
    SELECT 
    GroupID = ROW_NUMBER() OVER (PARTITION BY email ORDER BY id) FROM Person
)
DELETE temp WHERE GroupID > 1
/* 4.Second Highest Salary --連續兩個子查詢*/
SELECT MAX(salary) as SecondHighestSalary FROM Employee
WHERE salary != (SELECT MAX(salary) FROM Employee) 
/* 5. Group Sold Products By The Date--透過子查詢重新定義TABLE*/
SELECT sell_date,COUNT(sell_date) as num_sold,
STRING_AGG(product,',') AS products FROM (SELECT DISTINCT*FROM Activities) t
GROUP BY sell_date
/* 6.List the Products Ordered in a Period */
SELECT A.product_name,SUM(B.unit) as unit FROM Products A
LEFT JOIN Orders B ON B.product_id = A.product_id AND B.order_date BETWEEN '2020-02-01' AND '2020-02-29'
GROUP BY A.product_id,A.product_name
HAVING SUM(B.unit) >= 100
/* 7.Find Users With Valid E-Mails--透過雙重否定與LIKE*/
SELECT user_id, name, mail FROM Users
WHERE mail LIKE '[a-zA-Z]%@leetcode.com' AND LEFT(mail,LEN(mail)-13) NOT LIKE '%[^a-zA-Z0-9_.-]%'/*SELECT*/
/*1.Recyclable and Low Fat Products*/
SELECT product_id FROM Products WHERE low_fats ='Y' AND recyclable ='Y'
/* 2.Find Customer Referee */
SELECT [name] FROM Customer WHERE referee_id != 2 OR referee_id IS NULL
/* 3.Big Countries */
SELECT [name],[population],[area] FROM World WHERE area >= 3000000 OR population >= 25000000
/* 4.Article Views I */
SELECT DISTINCT author_id as id FROM Views WHERE author_id = viewer_id ORDER BY id
/* 5.Invalid Tweets */
SELECT tweet_id FROM Tweets WHERE LEN(content) > 15


/*Basic Joins*/
/*1.Replace Employee ID With The Unique Identifier */
SELECT B.unique_id,A.name FROM Employees A
LEFT JOIN EmployeeUNI B ON A.id = B.id
/* 2.Product Sales Analysis I */
SELECT B.product_name,A.year,A.price FROM Sales A
LEFT JOIN Product B ON A.product_id = B.product_id
/* 3.Customer Who Visited but Did Not Make Any Transactions */
SELECT A.customer_id,COUNT(A.visit_id) as count_no_trans FROM Visits A
LEFT JOIN Transactions B ON A.visit_id = B.visit_id
WHERE B.visit_id IS NULL
GROUP BY A.customer_id
/* 4.Rising Temperature */
SELECT a.id FROM Weather A
LEFT JOIN Weather B ON DATEADD(day,-1,A.recordDate) =B.recordDate
WHERE A.temperature > B.temperature
/* 5.Average Time of Process per Machine:注意:Step1:將初始時間、結束時間至同一列 */
SELECT A.machine_id,round(AVG(B.timestamp-A.timestamp),3) as processing_time FROM Activity A
INNER JOIN Activity B 
ON A.machine_id = B.machine_id AND A.process_id = B.process_id AND A.timestamp < B.timestamp
group by A.machine_id
/* 6.Employee Bonus */
SELECT A.name,B.bonus FROM Employee A
FULL JOIN Bonus B ON A.empId = B.empId
WHERE B.bonus < 1000 OR B.bonus IS NULL
/* 7.Students and Examinations */
SELECT A.student_id,A.student_name,B.subject_name,COUNT(C.student_id) as attended_exams FROM Students A
cross join Subjects B
FULL JOIN Examinations C ON A.student_id = C.student_id AND B.subject_name = C.subject_name
GROUP BY A.student_id,A.student_name,B.subject_name
/* 8.Managers with at Least 5 Direct Reports */
SELECT A.name FROM Employee A
LEFT JOIN Employee B ON A.id = B.managerId
GROUP BY A.id,A.name
HAVING COUNT(B.id) >= 5
/* 9.Confirmation Rate 使用CASE WHEN*/
SELECT A.user_id,
ROUND(AVG(CASE
        WHEN B.action = 'confirmed' THEN 1.0
        ELSE 0.0
        END),2) AS confirmation_rate
FROM Signups A
LEFT JOIN Confirmations B ON A.user_id = B.user_id
GROUP BY A.user_id



/*Basic Aggregate Functions*/
/* 1.Not Boring Movies*/
SELECT * FROM Cinema
WHERE id%2=1 AND [description] != 'boring'
ORDER BY rating DESC
/* 2.Average Selling Price  使用ISNULL*/
SELECT A.product_id,ISNULL(ROUND(SUM(A.price * B.units)*1.0/SUM(B.units)*1.0,2),0) as average_price FROM Prices A
LEFT JOIN UnitsSold B ON A.product_id = B.product_id AND (B.purchase_date BETWEEN A.start_date AND A.end_date)
GROUP BY A.product_id 
/* 3.Project Employees I */
SELECT A.project_id,ROUND(AVG(B.experience_years*1.0),2) as average_years FROM Project A
LEFT JOIN Employee B ON A.employee_id = B.employee_id
GROUP BY A.project_id
/* 4.Percentage of Users Attended a Contest 使用子查詢 */
SELECT A.contest_id,ROUND((COUNT(A.user_id)*1.0/(SELECT COUNT(user_id) FROM Users) * 1.0)*100,2) as percentage FROM Register A
LEFT JOIN Users B ON A.user_id = B.user_id
GROUP BY A.contest_id 
ORDER BY [percentage] DESC,contest_id ASC
/* 5.Queries Quality and Percentage */
SELECT query_name,
ROUND(AVG(rating*1.0/position*1.0),2) as quality,
ROUND((SUM(CASE WHEN rating < 3 THEN 1 ELSE 0 END)*1.0/COUNT(*)*1.0)*100,2) as poor_query_percentage
FROM Queries
WHERE query_name IS NOT NULL
GROUP BY query_name
ORDER BY quality ASC
/*6.Monthly Transactions I*/
SELECT 
LEFT(Convert(varchar,trans_date,21),7) as month,
country,
COUNT(amount) as trans_count,
SUM(CASE WHEN state = 'approved' THEN 1 ELSE 0 END) as approved_count,
SUM(amount) as trans_total_amount,
SUM(CASE WHEN state = 'approved' THEN amount ELSE 0 END) as approved_total_amount
FROM Transactions
GROUP BY LEFT(Convert(varchar,trans_date,21),7),country
ORDER BY LEFT(Convert(varchar,trans_date,21),7) ASC,country DESC
/*7.Immediate Food Delivery II*/
WITH CTE AS(
    SELECT 
    customer_id,
    order_date,
    customer_pref_delivery_date,
    (CASE WHEN order_date=customer_pref_delivery_date THEN 1 ELSE 0 END ) as schedule,
    ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY order_date ASC) as ROW_ID
    FROM Delivery
)

SELECT 
    ROUND((SUM(schedule)*100)/(COUNT(customer_id)*1.0),2)as immediate_percentage
FROM CTE
WHERE ROW_ID = 1

/* 8.Game Play Analysis IV */
with first_login as
(SELECT player_id,MIN(event_date) as first_login,DATEADD(day,1,MIN(event_date)) as second_login FROM Activity
GROUP BY player_id)

SELECT ROUND((COUNT(*) * 1.0) / (SELECT COUNT(distinct player_id) FROM Activity) * 1.0,2) as fraction FROM Activity A
JOIN first_login B ON A.event_date = B.second_login AND A.player_id = B.player_id

/*Basic Aggregate Functions*/
/* 1.User Activity for the Past 30 Days I */
SELECT activity_date as day,COUNT(DISTINCT user_id) as active_users FROM Activity
WHERE activity_date BETWEEN DATEADD(day,-29,'2019-07-27') AND '2019-07-27'
GROUP BY activity_date
/* 2.Number of Unique Subjects Taught by Each Teacher */
SELECT teacher_id,COUNT(DISTINCT subject_id) as cnt FROM Teacher
GROUP BY teacher_id
/* 3.Product Sales Analysis III */
SELECT product_id, year AS first_year, quantity, price FROM Sales 
WHERE (product_id, year) IN (
    SELECT product_id, MIN(year) AS year FROM Sales 
    GROUP BY product_id);
/* 4.Write your T-SQL query statement below */
SELECT Class FROM Courses
GROUP BY class
HAVING COUNT(class) >= 5
/* 5.Find Followers Count */
SELECT user_id,COUNT(follower_id) as followers_count FROM Followers
GROUP BY user_id
/* 6.Biggest Single Number*/
SELECT MAX(num) as num FROM MyNumbers
WHERE num IN (SELECT * FROM MyNumbers
GROUP BY num
HAVING COUNT(num) = 1)
/* 7.Customers Who Bought All Products--善用DISTINCT*/
SELECT customer_id FROM Customer
GROUP BY customer_id 
HAVING COUNT(DISTINCT product_key) = (SELECT COUNT(product_key) FROM Product) 


/*Advanced Select and Joins*/
/* 1.The Number of Employees Which Report to Each Employee 四捨五入要記得改成浮點數*/
SELECT A.employee_id,A.name,COUNT(B.employee_id) as reports_count,ROUND(AVG(B.age*1.0),0) as average_age FROM Employees A
LEFT JOIN Employees B ON A.employee_id=B.reports_to
GROUP BY  A.employee_id,A.name
HAVING COUNT(B.employee_id) >= 1
ORDER BY employee_id
/* 2.Primary Department for Each Employee */
SELECT employee_id,department_id FROM Employee
WHERE primary_flag = 'Y' OR employee_id IN (SELECT employee_id FROM Employee
GROUP BY employee_id
HAVING COUNT(department_id) = 1)
/* 3.Triangle Judgement */
SELECT x,y,z,(CASE 
WHEN x+y<=z then 'No'
WHEN y+z<=x then 'No'
WHEN x+z<=y then 'No'
ELSE 'Yes' END
) as triangle FROM Triangle 
/*4.Consecutive Numbers */
SELECT DISTINCT A.num as ConsecutiveNums FROM Logs A,Logs B,Logs C
WHERE A.id = B.id + 1 
AND B.id = C.id + 1 
AND A.num = B.num 
AND B.num = C.num
/* 5.Product Price at a Given Date --注意UNION與排序*/
SELECT product_id,
(First_value(new_price) OVER (PARTITION BY product_id ORDER BY change_date DESC )) as price FROM Products
WHERE change_date <= '2019-08-16'
UNION
SELECT product_id,10 FROM Products
GROUP BY product_id
HAVING MIN(change_date) > '2019-08-16'
/* 6.Last Person to Fit in the Bus --利用TOP(1)與SUM(weight)依序相加*/
with Second as
(SELECT person_id,person_name,[weight],turn, 
SUM(weight) OVER (ORDER BY turn ASC)as total FROM [Queue])

SELECT TOP(1) person_name FROM Second
WHERE total <= 1000
ORDER BY turn DESC
/* 7.Count Salary Categories */
SELECT 'Low Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income < 20000
UNION
SELECT 'Average Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income BETWEEN 20000 AND 50000
UNION
SELECT 'High Salary' as category,COUNT(account_id) as accounts_count FROM Accounts
WHERE income > 50000


/* Subqueries */
/* 1.Employees Whose Manager Left the Company */
SELECT employee_id FROM Employees
WHERE salary < 30000 AND manager_id NOT IN (SELECT employee_id FROM Employees)
ORDER BY employee_id 
/* 2.Exchange Seats --數據平移，LEAD()和LAG() */
SELECT (CASE WHEN ID%2=0 THEN ID-1
             WHEN ID%2=1 THEN LEAD(ID,1,ID) OVER (ORDER BY ID ASC) END) as id,student FROM Seat
ORDER BY id ASC
/* 3.Movie Rating */
WITH results1 AS(
    SELECT TOP(1) C.name as results FROM MovieRating A
LEFT JOIN Users C ON A.user_id = C.user_id
GROUP BY C.name
ORDER BY COUNT(A.rating) DESC,C.name ASC
),results2 AS(
    SELECT TOP(1) B.title as results FROM MovieRating A
LEFT JOIN Movies B ON A.movie_id = B.movie_id
WHERE A.created_at like '2020-02%'
GROUP BY B.title
ORDER BY AVG(A.rating + 0.00) DESC,B.title ASC
)

SELECT results FROM results1
UNION all
SELECT results FROM results2
/* 4.Restaurant Growth --Rows Between來加總特定區間的值*/
SELECT visited_on,
    SUM(SUM(amount)) OVER (ORDER BY visited_on ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) as amount,
    ROUND(CAST(SUM(SUM(amount)) OVER (ORDER BY visited_on ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) as float)/7.0,2) as average_amount
    FROM Customer
GROUP BY visited_on
ORDER BY visited_on
offset 6 ROWS
/* 5.Friend Requests II: Who Has the Most Friends */
WITH Answer AS (
    SELECT requester_id as id FROM RequestAccepted
    UNION ALL
    SELECT accepter_id as id FROM RequestAccepted
)
SELECT TOP(1) id,COUNT(id) as num FROM Answer
GROUP BY id
ORDER BY COUNT(id) DESC
/* 6.Investments in 2016 --窗口函數會計算與它相同的行數。*/
WITH Answer AS(
    SELECT
	*,
	COUNT(lat) OVER(PARTITION BY lat,lon) CountLatLon,
	COUNT(tiv_2015) OVER(PARTITION BY tiv_2015) CountT_2015
	FROM　Insurance 
)
SELECT ROUND(SUM(tiv_2016),2) as tiv_2016 FROM Answer
WHERE CountLatLon = 1 AND CountT_2015 >1

/* 7.Department Top Three Salaries --DENSE_RANK()資料排序w */
WITH Answer as(
SELECT B.name as Department,A.name as Employee,A.salary as Salary,
DENSE_RANK() OVER (PARTITION BY B.name ORDER BY A.salary DESC) as rank FROM Employee A
LEFT JOIN Department B ON A.departmentId = B.id
)

SELECT Department,Employee,Salary FROM Answer
WHERE rank <= 3
ORDER BY Department ASC,Salary DESC

/* Advanced String Functions / Regex / Clause*/
/* 1.Fix Names in a Table --透過UPPER調整字串*/
SELECT user_id,UPPER(LEFT(name,1))+LOWER(RIGHT(name,LEN(name)-1))as name FROM Users
ORDER BY user_id ASC
/* 2.Patients With a Condition*/
SELECT * FROM Patients
WHERE conditions LIKE '% DIAB1%' OR conditions LIKE 'DIAB1%'
/* 3.Delete Duplicate Emails --透過ROW_NUMBER()排序*/
with temp as(
    SELECT 
    GroupID = ROW_NUMBER() OVER (PARTITION BY email ORDER BY id) FROM Person
)
DELETE temp WHERE GroupID > 1
/* 4.Second Highest Salary --連續兩個子查詢*/
SELECT MAX(salary) as SecondHighestSalary FROM Employee
WHERE salary != (SELECT MAX(salary) FROM Employee) 
/* 5. Group Sold Products By The Date--透過子查詢重新定義TABLE*/
SELECT sell_date,COUNT(sell_date) as num_sold,
STRING_AGG(product,',') AS products FROM (SELECT DISTINCT*FROM Activities) t
GROUP BY sell_date
/* 6.List the Products Ordered in a Period */
SELECT A.product_name,SUM(B.unit) as unit FROM Products A
LEFT JOIN Orders B ON B.product_id = A.product_id AND B.order_date BETWEEN '2020-02-01' AND '2020-02-29'
GROUP BY A.product_id,A.product_name
HAVING SUM(B.unit) >= 100
/* 7.Find Users With Valid E-Mails--透過雙重否定與LIKE*/
SELECT user_id, name, mail FROM Users
WHERE mail LIKE '[a-zA-Z]%@leetcode.com' AND LEFT(mail,LEN(mail)-13) NOT LIKE '%[^a-zA-Z0-9_.-]%'

