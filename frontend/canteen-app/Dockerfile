FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN ls -la /app          # ← ADD THIS LINE (it will list all files)
RUN npm run build --prod

# FROM nginx:alpine
# COPY --from=build /app/dist/canteen-app /usr/share/nginx/html
# COPY nginx.conf /etc/nginx/conf.d/default.conf
# EXPOSE 80

FROM nginx:alpine
COPY dist/canteen-app /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
