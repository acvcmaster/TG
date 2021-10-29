function parametros_genetico_cs()
    data = load('parametros_genetico.csv');

    # 89:99 e 12:22

    x = data(12:22, 2) * 100;
    z = data(12:22, 3);

    plot(x, z);
    title("Ganho normalizado (algoritmo genético) [P = 52]", 'FontSize', 12);
    xlabel("Taxa de mutação (Mp)");
    ylabel("Ganho normalizado");
    pause();
endfunction