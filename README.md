# Ejercicio-Pagos


Contenido:

PAGO.API - Es un API REST el cual actua como servicio principal/orquestador.
PAGO.MICROSERVICE - Microservicio MINIMAL API REST el cual hace una operacion interna de la busqueda de productos.
PAGO.WEBSERVICE - Servicio web SOAP el cual funciona para la operacion interna de la actualizacion del estado.
PAGO.WEBSERVICE.WCF - Servicio WCF para la operacion interna de la consulta del estado.
.env - Archivo de declaracion de variables para la modificacion de claves
init.sql - Query inicial al construir el compose que creara las tablas y contenido inicial.
docker-compose.yml - Crea las imagenes de los programas y los ejecuta en contenedores los proyectos indicados y la base de datos.

Patron de diseño
El patrón de diseño utilizado de acuerdo a lo solicitado del ejercicio esta basado en una arquitectura de microservicios con orquestación centralizada, donde un servicio principal REST (PAGO.API) actúa como coordinador de la lógica de negocio, delegando tareas específicas a otros servicios de diferentes tipos; este servicio realiza operaciones de alta, actualización y consulta sobre una base de datos relacional SQL Server manteniendose ej ejecucion de contenedores tanto la BD como el servicio principal.

En cuanto a la operacion de alta se valida información recibida del cliente (como el monto total), el servicio principal consume un microservicio Minimal API para obtener informacion, validarla y posteriormente hacer la operacion correspondiente. En cuanto a la operacion de la actualización y consulta de pagos son delegadas a servicios SOAP tipo ASMX y WCF, publicados en IIS para el acceso de las peticiones enviadas desde el contenedor; esto debido a las restricciones de compatibilidad con Docker en Windows pero conectandose a la BD del contenedor.

Servicios desplegados en contenedores Linux:
PAGO.API
PAGO.MICROSERVICE
SQL SERVER

Servicios hospedados de forma local IIS:
PAGO.WEBSERVICE
PAGO.WEBSERVICE.WCF 

Diagrama Entidad-Relacion:

![image](https://github.com/user-attachments/assets/fa6a086d-451f-4470-8ff9-1b1f083437d6)

Manual de montaje y uso:

* Montar como sitio web en IIS la carpeta del IIS, quedando algo como:

![image](https://github.com/user-attachments/assets/df883d3b-e817-49cc-bdfa-bea41c87bb0d)

* Modificar el .env el IP_IIS por la ip local del IIS y si se quiere cambiar valores de las llaves (tomar en cuenta que hay que cambiar las variables tambien de web.config de los servicios SOAP)
  
* Ejecutar el docker compose con el comando: docker-compose up --build

* Hacer uso de las peticiones del postman collection para probar los servicios; las peticiones ubicados en la carpeta 'PETICIONES AL SERVICIO PRINCIPAL' son los principales para probar el funcionamiento
