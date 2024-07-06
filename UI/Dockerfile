# Usa una imagen de node como base 
FROM node:latest 

# Establece el directorio de trabajo dentro del contenedor 
WORKDIR /app 

# Copia los archivos del proyecto al contenedor 
COPY package*.json ./
# Copia los archivos del proyecto al contenedor 
COPY . . 

WORKDIR /app/my-nextjs-app 

# Instala las dependencias del proyecto 
RUN npm install 

COPY . .
EXPOSE 3000 
CMD npm run dev