FROM duunky/dunkyandfilscorporation:server-web-GIS
WORKDIR /
RUN mkdir /API
COPY Front /usr/share/nginx/html/
COPY installationDotNet.sh /
COPY start.sh /
COPY API_MJ/API /API
RUN chmod +x /installationDotNet.sh
RUN chmod +x /start.sh
RUN ./installationDotNet.sh
COPY default.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["/start.sh"]
